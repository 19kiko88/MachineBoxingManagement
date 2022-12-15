using MachineBoxingManagement.Services.Interfaces;
using MachineBoxingManagement.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MachineBoxingManagement.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using MachineBoxingManagement.Repositories.Models;
using System.Text.RegularExpressions;

namespace MachineBoxingManagement.Services
{
    public class BoxOutService : IBoxOutService
    {

        private readonly OServices.OraServiceClient _serviceClient;
        private readonly CAEDB01Context _caEDB01Context;
        private readonly DbContextOptionsBuilder<CAEDB01Context> _dbContextOptionsBuilder;

        public BoxOutService(string dbconn, string wcfconn)
        {
            _serviceClient = new OServices.OraServiceClient(OServices.OraServiceClient.EndpointConfiguration.BasicHttpBinding_IOraService, wcfconn);
            _dbContextOptionsBuilder = new DbContextOptionsBuilder<CAEDB01Context>();
            _dbContextOptionsBuilder.UseNpgsql(dbconn);
            _caEDB01Context = new CAEDB01Context(_dbContextOptionsBuilder.Options);
        }

        public List<Tuple<string, string, string>> GetInstockTime(List<string> pns, out string errMsg)
        {
            errMsg = "";
            var result = new List<Tuple<string, string, string>>();
            try
            {
                //every 200 pns per time in order not exceed the limit of sql cmd length.
                var tocnt = 200;
                var curTakeAmount = 0;

                while (curTakeAmount < pns.Count)
                {
                    var tmppns = pns.Skip(curTakeAmount).Take(tocnt).ToList();
                    curTakeAmount += tocnt;

                    var cmd = "select PN, CDT, SPEC_NOTE from EQP.EQP_QTY_NB_IDLE_INFO_V where " +
                    $"PN in ('{string.Join("', '", tmppns.Select(a => a.ToUpper()))}')";
                    var gresult = _serviceClient.GetEBSProData(cmd);
                    //<PN>(.+)<\/PN>(\r\n|\r|\n).+<CDT>(.+)<\/CDT>(\r\n|\r|\n).+<SPEC_NOTE>(.+)<\/SPEC_NOTE>
                    if (gresult.Nodes.Count == 2)
                    {
                        var pnMatches = Regex.Matches(gresult.Nodes[1].FirstNode.ToString(), @"<PN>(.+)(?:\r\n|\r|\n)?<\/PN>(?:\r\n|\r|\n).+<CDT>(.+)(?:\r\n|\r|\n)?<\/CDT>(?:\r\n|\r|\n).+<SPEC_NOTE>(.+)(?:\r\n|\r|\n)?<\/SPEC_NOTE>", RegexOptions.IgnoreCase);
                        foreach (Match pnMatch in pnMatches)
                        {
                            result.Add(Tuple.Create(pnMatch.Groups[1].Value, pnMatch.Groups[2].Value, pnMatch.Groups[3].Value));
                        }
                    }

                }

            }
            catch (Exception err)
            {

                errMsg = err.Message;
            }
            return result;
        }

        public List<string> GetPartNumberFromModel(string model)
        {
            var result = new List<string>();
            try
            {
                var queryString = "SELECT ITEM_NUMBER, DESCRIPTION " +
                    "FROM ODS.XX_INV_SYSTEM_ITEMS " +
                    "WHERE ITEM_NUMBER LIKE '90%' AND " +
                    $"DESCRIPTION LIKE '%//{model}-%'";
                var getResult = _serviceClient.GetEBSProDataAsync(queryString).Result.GetEBSProDataResult;

                if (getResult.Nodes.Count == 2)
                {
                    var pnMatches = Regex.Matches(getResult.Nodes[1].FirstNode.ToString(), @"<ITEM_NUMBER>(.+)<\/ITEM_NUMBER>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    foreach (Match pnMatch in pnMatches)
                    {
                        if (!result.Contains(pnMatch.Groups[1].Value))
                            result.Add(pnMatch.Groups[1].Value);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            

            return result;
        }

        public List<MachineBoxingInfoView> QueryMachines(QueryRule rule, out string errMsg, List<MachineBoxingInfo> sources = null)
        {
            var result = new List<MachineBoxingInfoView>();
            errMsg = "";
            try
            {

                var allLocations = _caEDB01Context.BoxingLocation.Select(a => a).ToList();
                var allBoxingOptions = _caEDB01Context.BoxingOption.Select(a => a).ToList();
                var allStatus = _caEDB01Context.BoxingStatus.Select(a => a).ToList();
                var allStyleNames = _caEDB01Context.BoxingStyle.Select(a => a.Name).ToList();

                IQueryable<MachineBoxingInfo> qresult = _caEDB01Context.MachineBoxingInfo.AsQueryable();

                if (sources == null)
                {
                    if (!string.IsNullOrEmpty(rule.Sd_BoxOut) || !string.IsNullOrEmpty(rule.Ed_BoxOut))
                    {//取出起或訖有填寫
                        var Dt_Sd_BoxOut = DateTime.TryParse(rule.Sd_BoxOut, out var dt3) ? dt3 : Convert.ToDateTime($"0001/01/01 00:00:00");
                        var Dt_Ed_BoxOut = DateTime.TryParse(rule.Ed_BoxOut, out var dt4) ? dt4.AddDays(1).AddMilliseconds(-1000) : Convert.ToDateTime($"9999/12/31 23:59:59");

                        qresult = qresult.Where(c => (c.TakeOutTime >= Dt_Sd_BoxOut && c.TakeOutTime <= Dt_Ed_BoxOut)).AsQueryable();
                    }

                    if (!string.IsNullOrEmpty(rule.PartNumber))
                    {
                        qresult = qresult.Where(c => c.PartNumber.ToUpper().StartsWith(rule.PartNumber.ToUpper())).AsQueryable();
                    }

                    if (!string.IsNullOrEmpty(rule.Model))
                    {
                        var pns = new List<string>();
                        try
                        {
                            pns = GetPartNumberFromModel(rule.Model);
                        }
                        catch (Exception)
                        {
                            pns = new List<string>() { "Error" };
                        }
                        finally
                        {
                            qresult = qresult.Where(a => pns.Select(b => b.ToUpper()).ToList().Contains(a.PartNumber.ToUpper())).AsQueryable();
                        }
                    }

                    if (rule.LocationIds.Count > 0)
                    {
                        qresult = qresult.Where(a => rule.LocationIds.Contains(a.BoxingLocationId)).AsQueryable();
                    }

                    if (rule.OptionIds.Count > 0)
                    {
                        qresult = qresult.Where(a => rule.OptionIds.Contains(a.BoxingOptionId)).AsQueryable();
                    }

                    if (rule.StyleIds.Count > 0)
                    {
                        qresult = qresult.Where(a => rule.StyleIds.Contains(a.BoxingStyleId)).AsQueryable();
                    }

                    if (rule.Statuses.Count > 0)
                    {
                        qresult = qresult.Where(a => rule.Statuses.Contains(a.StatusId)).AsQueryable();
                    }

                    switch (rule.BufferAreas)
                    {//轉換暫存區查詢條件：0 =>全部選, 1 => 非暫存區, 2=>暫存區, 3=>全選
                        case 1:
                            qresult = qresult.Where(c => c.BufferArea == false).AsQueryable();
                            break;
                        case 2:
                            qresult = qresult.Where(c => c.BufferArea == true).AsQueryable();
                            break;
                        default:
                            break;
                    }
                }

                if (sources != null && sources.Count > 0)
                {
                    qresult = sources.AsQueryable();
                }

                var List_qresult = qresult.ToList();
                if (List_qresult.Count > 0)
                {
                    var allcdt = GetInstockTime(List_qresult.Select(a => a.PartNumber).Distinct().ToList(), out string cdtError);

                    foreach (var r in List_qresult)
                    {
                        result.Add(new MachineBoxingInfoView
                        {
                            ID = r.Id,
                            PartNumber = r.PartNumber,
                            BufferArea = r.BufferArea,
                            Description = allcdt.Where(a => a.Item1.ToUpper() == r.PartNumber.ToUpper()).Select(a => a.Item3).FirstOrDefault(),
                            SSN = r.Ssn,
                            Operator = r.Operator,
                            OperationTime = r.OperateTime,
                            Model = allcdt.Where(a => a.Item1.ToUpper() == r.PartNumber.ToUpper()).Select(a => a.Item3.Split(new string[] { "//" }, StringSplitOptions.None).GetValue(1).ToString().Split('-').GetValue(0).ToString()).FirstOrDefault(),
                            Location = allLocations.Where(a => a.Id == r.BoxingLocationId).Select(a => a.Name).FirstOrDefault(),
                            BoxingName = r.BoxingName,
                            BoxingOption = allBoxingOptions.Where(a => a.Id == r.BoxingOptionId).Select(a => a.Name).FirstOrDefault(),
                            BoxingSerial = r.BoxingSerial,
                            StackLevel = r.StackLevel,
                            Status = allStatus.Where(a => a.Id == r.StatusId).Select(a => a.Name).FirstOrDefault(),
                            Takeouter = r.TakeOutor,
                            TakeoutTime = r.TakeOutTime,
                            InStockDate = allcdt.Where(a => a.Item1.ToUpper() == r.PartNumber.ToUpper()).Select(a => a.Item2).FirstOrDefault()
                        });
                    }

                    var Dt_Sd_BoxIn = DateTime.TryParse(rule.Sd_BoxIn, out var dt1) ? dt1 : Convert.ToDateTime($"0001/01/01 00:00:00");
                    var Dt_Ed_BoxIn = DateTime.TryParse(rule.Ed_BoxIn, out var dt2) ? dt2.AddDays(1).AddMilliseconds(-1000) : Convert.ToDateTime($"9999/12/31 23:59:59");
                    result = result.Where(c => Convert.ToDateTime(c.OperationTime) >= Dt_Sd_BoxIn && Convert.ToDateTime(c.OperationTime) <= Dt_Ed_BoxIn).ToList();
                }

            }
            catch (Exception err)
            {
                errMsg = err.Message;
            }

            return result;
        }

        public List<int> SaveMachineBufferArea(Dictionary<int, bool> ids, string operater, out string errMsg)
        {
            errMsg = "";
            var result = new List<int>();
            var now = DateTime.Now;

            var currentBoxLocationId = 0;
            var currentBoxNmae = "";
            var currentBoxSerial = 0;

            using (var transac = _caEDB01Context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var id in ids.Keys)
                    {
                        var machine = _caEDB01Context.MachineBoxingInfo.Where(a => a.Id == id).FirstOrDefault();
                        if (machine != null)
                        {
                            /*找出同地點 & 箱名 & 箱號的機台，不管是否為查詢條件的料號，變更暫存區狀態*/
                            if (currentBoxNmae != machine.BoxingName && currentBoxSerial != machine.BoxingSerial)
                            {
                                currentBoxLocationId = machine.BoxingLocationId;
                                currentBoxNmae = machine.BoxingName;
                                currentBoxSerial = machine.BoxingSerial;

                                var machineInSameBoxButSerialNotSame = _caEDB01Context.MachineBoxingInfo.Where(c => c.StatusId == 666 && c.BoxingLocationId == currentBoxLocationId && c.BoxingName == currentBoxNmae && c.BoxingSerial == currentBoxSerial).ToList();
                                machineInSameBoxButSerialNotSame.ForEach(c =>
                                {
                                    result.Add(id);
                                    c.BufferArea = !c.BufferArea;
                                    c.Operator = operater;
                                    c.OperateTime = now;
                                });
                            }
                        }
                    }
                    _caEDB01Context.SaveChanges();
                    transac.Commit();
                }
                catch (Exception err)
                {
                    transac.Rollback();
                    errMsg = err.Message;
                }
            }

            return result;
        }

        public List<int> TakeoutMachines(List<int> ids, string takeouter, out string errMsg)
        {
            errMsg = "";
            var result = new List<int>();
            var now = DateTime.Now;
            using (var transac = _caEDB01Context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var id in ids)
                    {
                        var machine = _caEDB01Context.MachineBoxingInfo.Where(a => a.Id == id).FirstOrDefault();
                        if (machine != null)
                        {
                            result.Add(id);
                            machine.StatusId = 777;
                            machine.TakeOutor = takeouter;
                            machine.TakeOutTime = now;
                        }
                    }
                    _caEDB01Context.SaveChanges();
                    transac.Commit();
                }
                catch (Exception err)
                {
                    transac.Rollback();
                    errMsg = err.Message;
                }
            }
                
            return result;
        }
    }
}
