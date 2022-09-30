using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

using MachineBoxingManagement.Repositories.Data;
using MachineBoxingManagement.Repositories.Models;
using MachineBoxingManagement.Web.Services.Interfaces;
using MachineBoxingManagement.Services.Models;
using UniversalLibrary.Models;
using MachineBoxingManagement.Web.Models.BoxIn;
using MachineBoxingManagement.Web.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using MachineBoxingManagement.Web.Services.Common.Extensions;
using System.Threading.Tasks;

namespace MachineBoxingManagement.Web.Services.Implements
{
    public class BoxInService : IBoxInService
    {
        private readonly CAEDB01Context _context;
        private readonly IConfiguration _configuration;
        private readonly IBoxOutService _boxoutService;
        private readonly MachineBoxingManagement.Services.BoxInService _winformBoxinService;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //private ISession _session => _httpContextAccessor.HttpContext.Session;
        private const string SessionKey_PN_Temp_Data = "PN_Temp_Data";
        private const int Max_Box_Qty = 20;



        public BoxInService(CAEDB01Context context, IConfiguration config, IBoxOutService boxOutService/*, IHttpContextAccessor httpContextAccessor*/)
        {
            _context = context;
            _configuration = config;
            _boxoutService = boxOutService;
            _winformBoxinService = new MachineBoxingManagement.Services.BoxInService(_configuration.GetValue<string>("ConnectionStrings:CAEDB01Connection"), _configuration.GetValue<string>("ConnectionStrings:CAEServiceConnection"));
            //_httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// PartNumber檢核
        /// </summary>
        /// <param name="partNumber"></param>
        /// <param name="machineOption"></param>
        /// <param name="machineStyle"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public async Task<Tuple<PartNumber_Model_Desc, string>> ProcessingPN(string userName, string partNumber, string ssn, int location, int machineOption, int machineStyle)
        {
            var errorMsg = string.Empty;
            var RePartNumber = partNumber.ToUpper().Trim();
            PartNumber_Model_Desc Pn_Model_Desc = new PartNumber_Model_Desc() {};

            var instockInfo = _boxoutService.GetInstockTime(RePartNumber, out string instockErr);

            if (instockInfo != null && string.IsNullOrEmpty(instockErr))
            {
                if (!System.DateTime.TryParse(instockInfo.CDT, out DateTime isd))
                {
                    errorMsg = $"無法取得料號入庫日資訊，請再重試看看，訊息:{instockErr}";
                    return new Tuple<PartNumber_Model_Desc, string>(null, errorMsg);
                }
                else
                {
                    Pn_Model_Desc.InsDate = isd.ToString("yyyy-MM-dd HH:mm:ss");
                }

                Pn_Model_Desc.Desc = instockInfo.SPEC_NOTE;

                //切割SPEC_NOTE取得Model
                if (!string.IsNullOrEmpty(instockInfo.SPEC_NOTE))
                {
                    //第一次字串切割('//')
                    string[] Ary_Split_1 = instockInfo.SPEC_NOTE.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
                    if (Ary_Split_1.Length > 0)
                    {
                        Pn_Model_Desc.Model = Ary_Split_1[1];

                        //第二次字串切割('/')
                        Pn_Model_Desc.Model = Pn_Model_Desc.Model.IndexOf('/') > 0 ? Pn_Model_Desc.Model.Split('/')[0] : Pn_Model_Desc.Model;

                        //第三次字串切割('-')
                        Pn_Model_Desc.Model = Pn_Model_Desc.Model.IndexOf('-') > 0 ? Pn_Model_Desc.Model.Split('-')[0] : Pn_Model_Desc.Model;
                    }
                }

                //1:一般NPI機台 11:海運
                if (machineOption == 1 || machineOption == 11)
                {
                    var pnXstatus = _winformBoxinService.GetPnXStatus(new List<string> { RePartNumber });

                    if (!pnXstatus.Success)
                    {
                        errorMsg = $"無法取得料號OA相關訊息，請再重試看看，訊息:{pnXstatus.Message}";
                        return new Tuple<PartNumber_Model_Desc, string>(null, errorMsg);
                    }

                    Pn_Model_Desc.ToOA = pnXstatus.Content.FirstOrDefault()?.ToOA;
                    Pn_Model_Desc.TearDown = pnXstatus.Content.FirstOrDefault()?.TearDown;
                    Pn_Model_Desc.MbResale = pnXstatus.Content.FirstOrDefault()?.MbResale;

                    if (Pn_Model_Desc.ToOA == null || Pn_Model_Desc.MbResale == null)
                    {
                        errorMsg = $"料號尚未登入OA或拆解相關訊息，請先上傳再繼續，訊息:{pnXstatus.Message}";
                        return new Tuple<PartNumber_Model_Desc, string>(null, errorMsg);
                    }
                }

                if (Pn_Model_Desc.ToOA.HasValue && Pn_Model_Desc.ToOA.Value)
                {
                    if (Convert.ToDateTime(Pn_Model_Desc.InsDate).AddYears(1) < DateTime.Now)
                    {
                        //已過一年可轉OA
                        machineStyle = 11; //待燒OA;
                    }
                }
                else if (Pn_Model_Desc.TearDown.HasValue && Pn_Model_Desc.TearDown.Value)
                {
                    if (Pn_Model_Desc.MbResale.HasValue)
                    {
                        machineStyle = 21; //可拆解可轉賣
                    }
                    else
                    {
                        machineStyle = 31; //可拆解不可轉賣
                    }
                }

                //取得裝箱系列(箱名)
                var BoxSeries = GetBoxingName(partNumber, Pn_Model_Desc.Model, machineOption, machineStyle, Convert.ToDateTime(Pn_Model_Desc.InsDate), out errorMsg);
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    return new Tuple<PartNumber_Model_Desc, string>(null, errorMsg);
                }

                //Setting P/N Data
                Pn_Model_Desc.Part_Number = partNumber;//P/N
                if (!string.IsNullOrEmpty(ssn))
                {
                    Pn_Model_Desc.SSN = ssn;//SSN
                }
                Pn_Model_Desc.Boxing_Location_Id = location;//庫房
                Pn_Model_Desc.Boxing_Location_Cn = _context.BoxingLocation.Where(c => c.Id == location).FirstOrDefault()?.Name; ;//庫房中文
                Pn_Model_Desc.Boxing_Option_Id = Convert.ToInt32(machineOption);//機台選項
                Pn_Model_Desc.Boxing_Option_Cn = _context.BoxingOption.Where(c => c.Id == machineOption).FirstOrDefault()?.Name;//機台選項中文
                Pn_Model_Desc.Boxing_Style_Id = Convert.ToInt32(machineStyle);//樣式選項
                Pn_Model_Desc.Boxing_Series = BoxSeries;//裝箱系列(箱名)
                Pn_Model_Desc.Status_Id = 666;
                Pn_Model_Desc.Operator = userName;
                Pn_Model_Desc.OperateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                errorMsg = $"無法取得料號入庫日資訊，請再重試看看，訊息:{instockErr}";
                return new Tuple<PartNumber_Model_Desc, string>(null, errorMsg);
            }

            return new Tuple<PartNumber_Model_Desc, string>(Pn_Model_Desc, errorMsg);
        }

        /// <summary>
        /// Get Machine In Boxing Info，原Winform GegMIB()
        /// </summary>
        /// <param name="locationID"></param>
        /// <param name="boxingName"></param>
        /// <returns></returns>
        public async Task<Tuple<Stocking_Info, string>> Get_Machine_In_Box_Info(string boxingSeries, int boxingLocation, bool saveTemp = false, List<PartNumber_Model_Desc>? tempDataList = null)
        {
            var errorMsg = string.Empty;
            var Res = new Stocking_Info();
            var Boxing_Db_Qty = 0;
            var Boxing_Temp_Qty = 0;

            //透過裝箱系列(箱名)由db取得目前使用到的箱號
            var Boxing_Series_Result = GetBoxingSerialInfo(boxingSeries, boxingLocation, out errorMsg);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                return new Tuple<Stocking_Info, string>(null, errorMsg);
            }

            Boxing_Db_Qty = Boxing_Series_Result.Qty;

            if (tempDataList == null || tempDataList.Count == 0 ||
                (tempDataList != null && tempDataList.Count > 0 && !tempDataList.Where(c => c.Boxing_Series == boxingSeries).Any()/*箱號可以手動變更，手動輸入的箱號有可能會不存在於暫存清單*/)
                )
            {
                //箱內數量是否超過20
                var MoreThanTwenty = Boxing_Db_Qty + Boxing_Temp_Qty >= Max_Box_Qty ? true : false;

                Res = new Stocking_Info
                {
                    Boxing_Db_Qty = MoreThanTwenty ? 0 : Boxing_Db_Qty,
                    Boxing_Temp_Qty = MoreThanTwenty ? 
                        saveTemp ? 1 : 0 
                        : Boxing_Temp_Qty,
                    Qty = MoreThanTwenty ? 0 : Boxing_Db_Qty + Boxing_Temp_Qty,//箱內數量
                    Turtle_Level = MoreThanTwenty ? 1 : Boxing_Series_Result.Turtle_Level//烏龜車層數
                };

                if (MoreThanTwenty)
                {
                    Res.Box_Serial = Boxing_Series_Result.Box_Serial + 1;
                }
                else
                {
                    Res.Box_Serial = Boxing_Series_Result.Box_Serial == 0 ? 1 : Boxing_Series_Result.Box_Serial;
                }
            }
            else
            {//Current_Box_Info有資料，從List_Temp_Data計算最新Stocking Info

                //取得暫存資料列目前最大箱號 & 數量(用箱號group by的count算出)
                var Current_Box_Info = tempDataList
                    .Where(c => c.Boxing_Series == boxingSeries && c.Boxing_Location_Id == boxingLocation)
                    .GroupBy(g => g.Boxing_Serial)
                    .Select(c => new { Box_Serial = c.Key, Box_Serial_Qty = c.Count() })
                    .OrderByDescending(c => c.Box_Serial)
                    .FirstOrDefault();

                //取得目前最大箱號的烏龜車層數
                var Turtle_Level = tempDataList
                    .Where(c => c.Boxing_Series == boxingSeries && c.Boxing_Location_Id == boxingLocation && c.Boxing_Serial == Current_Box_Info.Box_Serial)
                    .OrderByDescending(c => c.Boxing_Serial)
                    .FirstOrDefault()?.Turtle_Level;

                if (Current_Box_Info != null && Turtle_Level != null)
                {
                    //暫存資料的最大箱號跟db查詢出來的不一樣的話，db箱號歸0
                    Boxing_Db_Qty = Current_Box_Info.Box_Serial != Boxing_Series_Result.Box_Serial ? 0 : Boxing_Db_Qty;

                    //箱內數量是否超過20
                    Boxing_Temp_Qty = Current_Box_Info.Box_Serial_Qty;
                    var MoreThanTwenty = Boxing_Db_Qty + Boxing_Temp_Qty >= Max_Box_Qty ? true : false;

                    Res = new Stocking_Info() { 
                        Box_Serial = MoreThanTwenty ? Current_Box_Info.Box_Serial + 1 : Current_Box_Info.Box_Serial,//箱號(超過20就+1)
                        Boxing_Db_Qty = MoreThanTwenty ? 0 : Boxing_Db_Qty,
                        Boxing_Temp_Qty = MoreThanTwenty ?
                            saveTemp ? 1 : 0 
                            : Boxing_Temp_Qty,
                        Qty = MoreThanTwenty ? 0 : Boxing_Db_Qty + Boxing_Temp_Qty,//箱內數量
                        Turtle_Level = MoreThanTwenty ? 1 : Turtle_Level.Value//烏龜車層數
                    };
                }
            }

            return new Tuple<Stocking_Info, string>(Res, errorMsg);
        }

        /// <summary>
        /// 取得指定箱號的裝箱儲位相關資訊(箱號(Serial), 箱內數量, 烏龜車層數)
        /// </summary>
        /// <param name="boxingSeries"></param>
        /// <param name="boxingLocation"></param>
        /// <param name="boxSerial"></param>
        /// <param name="tempDataList"></param>
        /// <returns></returns>
        public async Task<Tuple<Stocking_Info, string>> Get_Machine_In_Box_Info_By_BoxSerial(string boxingSeries, int boxingLocation, int boxSerial, List<PartNumber_Model_Desc>? tempDataList = null)
        {
            var errorMsg = string.Empty;
            var Res = new Stocking_Info();
            var Boxing_Db_Qty = 0;
            var Boxing_Temp_Qty = 0;

            //透過裝箱系列(箱名)由db取得目前使用到的箱號
            var Boxing_Series_Result = GetBoxingSerialInfo(boxingSeries, boxingLocation, out errorMsg, boxSerial);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                return new Tuple<Stocking_Info, string>(null, errorMsg);
            }
            Boxing_Db_Qty = Boxing_Series_Result.Qty;

            //取得暫存資料列目前箱號 & 數量(用箱號group by的count算出)
            var Current_Box_Info = tempDataList?
                .Where(c => c.Boxing_Series == boxingSeries && c.Boxing_Location_Id == boxingLocation && c.Boxing_Serial == boxSerial)
                .GroupBy(g => g.Boxing_Serial)
                .Select(c => new { Box_Serial = c.Key, Box_Serial_Qty = c.Count() })
                .OrderByDescending(c => c.Box_Serial)
                .FirstOrDefault();
            Boxing_Temp_Qty = Convert.ToInt32(Current_Box_Info?.Box_Serial_Qty);

            //取得目前箱號的烏龜車層數
            var Turtle_Level = tempDataList?
                .Where(c => c.Boxing_Series == boxingSeries && c.Boxing_Location_Id == boxingLocation && c.Boxing_Serial == boxSerial && c.Turtle_Level > 0)               
                .FirstOrDefault()?.Turtle_Level;
            
            if (!Turtle_Level.HasValue)
            {
                Turtle_Level = Boxing_Series_Result.Turtle_Level;
            }

            Res = new Stocking_Info()
            {
                Box_Serial = boxSerial,
                Boxing_Db_Qty = Boxing_Db_Qty,
                Boxing_Temp_Qty = Boxing_Temp_Qty,
                Qty = Boxing_Db_Qty + Boxing_Temp_Qty,//箱內數量
                Turtle_Level = (int)Turtle_Level.Value//烏龜車層數
            };

            return new Tuple<Stocking_Info, string>(Res, errorMsg);
        }

        /// <summary>
        /// 取得裝箱系列資訊
        /// 由WCF Service取得裝箱系列相關資訊，重新包裝Winform GetBoxingName()為強型別
        /// </summary>
        /// <param name="partNumber"></param>
        /// <param name="model"></param>
        /// <param name="machineOption"></param>
        /// <param name="machineStyle"></param>
        /// <param name="insDate"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public string GetBoxingName(string partNumber, string model, int machineOption, int machineStyle, DateTime insDate, out string errorMsg)
        {
            errorMsg = string.Empty;
            var Res = string.Empty;

            var CN_MachineOption = _context.BoxingOption.Where(c => c.Id == machineOption).FirstOrDefault()?.Name;
            var CN_MachineStyle = _context.BoxingStyle.Where(c => c.Id == machineStyle).FirstOrDefault()?.Name;

            var bnameResult = _winformBoxinService.GetBoxingName(partNumber, model, CN_MachineOption, CN_MachineStyle, insDate);

            if (bnameResult.Success)
            {
                Res = bnameResult.Content;
            }
            else
            {
                errorMsg = $"P/N內容可能錯誤，訊息:{bnameResult.Message}";
            }

            return Res;
        }

        /// <summary>
        /// 取得裝箱系列相關資訊
        /// 由WCF Service取得裝箱系列相關資訊，重新包裝Winform GetBoxingSerialInfo()為強型別
        /// </summary>
        /// <param name="boxingSeries">裝箱系列(箱名)</param>
        /// <param name="boxingLocationId">庫房id</param>
        /// <param name="specifySerialNo">指定箱號</param>
        /// <returns></returns>
        public Stocking_Info GetBoxingSerialInfo(string boxingSeries, int boxingLocationId, out string errorMsg, int? specifySerialNo = null) 
        {
            Stocking_Info Res = null;
            errorMsg = string.Empty;
            //由WCF Service取得裝箱系列相關資訊
            var Boxing_Series_Result = _winformBoxinService.GetBoxingSerialInfo(boxingSeries, boxingLocationId, specifySerialNo);
            if (!Boxing_Series_Result.Success)
            {
                errorMsg = $"取箱號失敗，訊息:{Boxing_Series_Result.Message}";
            }
            else
            {
                Res = new Stocking_Info
                {
                    Box_Serial = Boxing_Series_Result.Content.Item1,//箱號(Serial)
                    Qty = Boxing_Series_Result.Content.Item2,//Item2:箱內數量
                    Turtle_Level = Boxing_Series_Result.Content.Item3//, Item3:烏龜車層數                
                };
            }

            return Res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> SaveBoxingInfos(List<PartNumber_Model_Desc> data)
        {
            var Res = 0;
            var ListData = new List<MachineBoxingInfo>();

            data.ForEach(item =>
            {
                var Data = new MachineBoxingInfo()
                {
                    PartNumber = item.Part_Number,
                    BufferArea = item.Is_Buffer_Area,
                    Ssn = item.SSN,
                    BoxingLocationId = item.Boxing_Location_Id,
                    BoxingOptionId = item.Boxing_Option_Id,
                    BoxingStyleId = item.Boxing_Style_Id,
                    BoxingName = item.Boxing_Series,
                    BoxingSerial = item.Boxing_Serial,
                    StackLevel = item.Turtle_Level,
                    StatusId = item.Status_Id,
                    Operator = item.Operator,
                    OperateTime = Convert.ToDateTime(item.OperateTime)
                };
                ListData.Add(Data);
            });

            Res = _winformBoxinService.SaveBoxingInfos(ListData).Content;

            return Res;
        }
    }
}
