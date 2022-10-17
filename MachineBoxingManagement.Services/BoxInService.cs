using MachineBoxingManagement.Repositories.Models;
using MachineBoxingManagement.Repositories.Data;
using MachineBoxingManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UniversalLibrary.Models;
using System.Linq;
using System.Text.RegularExpressions;
using MachineBoxingManagement.Services.Models;

namespace MachineBoxingManagement.Services
{
    public class BoxInService : IBoxInService
    {
        private readonly CAEService.OraServiceClient _serviceClient;
        private readonly CAEDB01Context _caEDB01Context;
        private readonly DbContextOptionsBuilder<CAEDB01Context> _dbContextOptionsBuilder;
        private readonly BoxOutService _boxOutService;

        public BoxInService(string dbconn, string wcfconn)
        {
            _serviceClient = new CAEService.OraServiceClient(CAEService.OraServiceClient.EndpointConfiguration.BasicHttpBinding_IOraService, wcfconn);
            _dbContextOptionsBuilder = new DbContextOptionsBuilder<CAEDB01Context>();
            _dbContextOptionsBuilder.UseNpgsql(dbconn);
            _caEDB01Context = new CAEDB01Context(_dbContextOptionsBuilder.Options);
            _boxOutService = new BoxOutService(dbconn, wcfconn);
        }

        public Result<Tuple<int, int, int>> GetBoxingSerialInfo(string boxingName, int boxingLocationId, int? specifyBoxSerial = null)
        {
            var result = new Result<Tuple<int, int, int>>
            {
                Success = true
            };

            try
            {
                var boxserial = _caEDB01Context.MachineBoxingInfo.Where(a => a.BoxingName == boxingName && a.BoxingLocationId == boxingLocationId && a.StatusId == 666).Select(a => new { a.BoxingSerial, a.StackLevel }).ToList().GroupBy(a => a.BoxingSerial).OrderByDescending(a => a.Key);
                if (specifyBoxSerial.HasValue)
                {
                    boxserial = boxserial.Where(c => c.Key == specifyBoxSerial).OrderByDescending(c => c.Key);
                }
                if (boxserial.Count() == 0)
                    result.Content = Tuple.Create(0, 0, 1);
                else
                    result.Content = Tuple.Create(boxserial.FirstOrDefault().Key, boxserial.FirstOrDefault().Count(),boxserial.FirstOrDefault().FirstOrDefault().StackLevel.Value);
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
                result.Exception = err;
            }
            return result;
        }

        public Result<string> GetBoxingName(string partnumber, string model, string BoxingOption, string BoxingStyle, DateTime instockTime)
        {
            var result = new Result<string>
            {
                Success = true
            };

            try
            {
                var boxName = BoxingOption.Replace("一般NPI機台", "");
                var pnkey = partnumber.Split('-').GetValue(1).ToString().Substring(0, 1);
                var boxstage = _caEDB01Context.BoxingStageRule.Where(a => Regex.IsMatch(pnkey, a.Rule, RegexOptions.IgnoreCase)).Select(a => a.Name).FirstOrDefault();
                
                var boxsSeries = _caEDB01Context.BoxingSeriesRule.Where(a => Regex.IsMatch(model, a.Rule, RegexOptions.IgnoreCase)).Select(a => a.Name).FirstOrDefault() ?? "其他";
                var instockSession = $"{instockTime.Year}{(instockTime.Month > 6 ? "下" : "上")}";

                switch (BoxingOption)
                {
                    case "一般NPI機台":
                    case String a when a.StartsWith("海運"):
                        if (boxstage == "MP")
                        {
                            result.Success = false;
                            result.Message = $"{partnumber}為MP料號, 請手動選取MP選項進行裝箱";
                            return result;
                        }
                        switch (BoxingStyle)
                        {
                            case "":
                                switch (BoxingOption)
                                {
                                    case "一般NPI機台":
                                        switch (boxstage)
                                        {
                                            case "ER":
                                                boxName += $"{boxstage}_{instockSession}";
                                                break;
                                            case "PR":
                                                boxName += $"{boxsSeries}_{boxstage}";
                                                break;
                                        }
                                        break;
                                    case String b when b.StartsWith("海運"):
                                        boxName += $"_{boxstage}";
                                        break;
                                }

                                break;
                            case "待燒OA":
                                boxName = $"{boxName}{BoxingStyle}";
                                break;
                            default:
                                boxName = $"{boxName}-{boxsSeries}{BoxingStyle}".TrimStart('-');
                                break;
                        }
                        break;
                    case String a when a.StartsWith("Fail拆解機台"):
                        if (boxstage == "MP")
                        {
                            boxstage = "PR";//MP歸類為PR_20221012 from Hank
                        }
                        var strInstockTime = instockTime.ToString("yyyy/M/dd");
                        boxName = $"Fail-{boxstage}-{instockSession}";
                        break;
                    default:
                        
                        break;
                }

                result.Content = boxName;
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
                result.Exception = err;
            }

            return result;
        }

        public Result<List<BoxingLocation>> EnumBoxingLocations()
        {
            var result = new Result<List<BoxingLocation>>
            {
                Success = true
            };
            try
            {
                result.Content = _caEDB01Context.BoxingLocation.Select(a => a).ToList();
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
                result.Exception = err;
            }

            return result;
        }

        public Result<List<BoxingOption>> EnumBoxingOptions()
        {
            var result = new Result<List<BoxingOption>>
            {
                Success = true
            };
            try
            {
                result.Content = _caEDB01Context.BoxingOption.Select(a => a).ToList();
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
                result.Exception = err;
            }

            return result;
        }

        public Result<int> SaveBoxingInfos(List<MachineBoxingInfo> infos)
        {
            var result = new Result<int>
            {
                Success = true
            };
            try
            {
                //clear id && change db stack lvl / bufferarea
                //var chkedItem = new List<string>();

                foreach (var info in infos)
                {
                    //    if (!chkedItem.Contains($"{info.BoxingLocationId}^{info.BoxingName}^{info.BoxingSerial}"))
                    //    {
                    //        var records = _caEDB01Context.MachineBoxingInfo.Where(a => a.BoxingLocationId == info.BoxingLocationId && a.BoxingName == info.BoxingName && a.BoxingSerial == info.BoxingSerial);
                    //        foreach (var record in records)
                    //        {
                    //            record.StackLevel = info.StackLevel;
                    //            record.BufferArea = info.BufferArea;
                    //        }
                    //        chkedItem.Add($"{info.BoxingLocationId}^{info.BoxingName}^{info.BoxingSerial}");
                    //    }
                    //    info.Id = 0;

                    _caEDB01Context.MachineBoxingInfo.Add(info);
                    result.Content += _caEDB01Context.SaveChanges();
                }

                //_caEDB01Context.MachineBoxingInfo.AddRange(infos);
                //result.Content = _caEDB01Context.SaveChanges();
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
                result.Exception = err;
            }
            return result;
        }

        public Result<List<BoxingStyle>> EnumBoxingStyles()
        {
            var result = new Result<List<BoxingStyle>>
            {
                Success = true
            };
            try
            {
                result.Content = _caEDB01Context.BoxingStyle.Select(a => a).ToList();
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
                result.Exception = err;
            }

            return result;
        }

        public Result<List<Tuple<string, string, string, DateTime?>>> GetPnModelDesc(List<string> partnumbers)
        {

            var result = new Result<List<Tuple<string, string, string, DateTime?>>>
            {
                Success = true,
                Content = new List<Tuple<string, string, string, DateTime?>>()
            };

            if (partnumbers.Count == 0) return result;

            try
            {

                var pnResult = _boxOutService.GetInstockTime(partnumbers, out string instockerr);

                if (!string.IsNullOrEmpty(instockerr))
                {
                    result.Success = false;
                    result.Message = instockerr;
                }
                else
                {
                    result.Content = pnResult.Select(a => Tuple.Create(
                        a.Item1,
                        a.Item3,
                        a.Item3.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries).GetValue(1).ToString().Split('/').GetValue(0).ToString().Split('-').GetValue(0).ToString(),
                        DateTime.TryParse(a.Item2, out DateTime dt) ? dt : (DateTime?)null
                        )).ToList();
                }
                
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
                result.Exception = err;
            }

            return result;

            
        }

        public Result<List<Tuple<string, string>>> GetPnOaCategory(List<string> partnumbers)
        {
            var result = new Result<List<Tuple<string, string>>>
            {
                Success = true,
                Content = new List<Tuple<string, string>>()
            };

            try
            {

                result.Content = _caEDB01Context.OaSpec.Where(a => partnumbers.Contains(a.PartNumber.ToUpper())).Select(
                    a => Tuple.Create(a.PartNumber, a.Category)
                    ).ToList();

            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
                result.Exception = err;
            }

            return result;
        }

        public Result<List<PnXStatus>> GetPnXStatus(List<string> partnumbers)
        {
            var result = new Result<List<PnXStatus>>
            {
                Success = true
            };

            try
            {
                foreach (var pn in partnumbers)
                {
                    PnXStatus pnStatus = new PnXStatus
                    {
                        PartNumber = pn
                    };

                    var mch = _caEDB01Context.MachineConfirmHistory.Where(a => a.PartNumber.ToLower() == pn.ToLower()).OrderByDescending(a => a.UpdateTime).FirstOrDefault();

                    if (mch != null)
                    {
                        pnStatus.TearDown = ConvertStrToBool(mch.CanTeardown);
                        pnStatus.ToOA = ConvertStrToBool(mch.ToOa);
                    }

                    var oaspec = _caEDB01Context.OaSpec.Where(a => a.PartNumber.ToLower() == pn.ToLower()).OrderByDescending(a => a.UpdateTime).FirstOrDefault();

                    if (oaspec != null)
                    {
                        pnStatus.MbResale = ConvertStrToBool(oaspec.Mbresale);
                    }
                    result.Content.Add(pnStatus);
                }

            }
            catch (Exception err)
            {

                result.Success = false;
                result.Message = err.Message;
                result.Exception = err;
            }


            return result;
        }

        private bool? ConvertStrToBool(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            if (str.StartsWith("是") ||
                str.StartsWith("可") ||
                str.ToUpper().StartsWith("Y"))
            {
                return true;
            }
            else if (str.StartsWith("否") ||
                str.ToUpper().StartsWith("N"))
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        public Result<List<MachineBoxingInfo>> GetDBBoxes(List<MachineUniBoxingProp> boxes)
        {
            var result = new Result<List<MachineBoxingInfo>>
            {
                Success = true,
                Content = new List<MachineBoxingInfo>()
            };

            try
            {
                foreach (var box in boxes)
                {
                    var matchedboxes = _caEDB01Context.MachineBoxingInfo.Where(a => a.BoxingLocationId == box.BoxingLocationID &&
                        a.BoxingName == box.BoxingName && a.BoxingSerial == box.BoxingSerial).ToList();
                    if (matchedboxes.Any())
                        result.Content.AddRange(matchedboxes);
                }
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
                result.Exception = err;
            }
            return result;
        }
    }
}
