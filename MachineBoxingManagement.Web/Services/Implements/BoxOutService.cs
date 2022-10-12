using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;

using MachineBoxingManagement.Repositories.Data;
using MachineBoxingManagement.Repositories.Models;
using MachineBoxingManagement.Web.Services.Interfaces;
using MachineBoxingManagement.Services.Models;
using UniversalLibrary.Models;
using MachineBoxingManagement.Web.Models.Dto;
using MachineBoxingManagement.Services.Models;
using System.Collections.Generic;

namespace MachineBoxingManagement.Web.Services.Implements
{
    public class BoxOutService: IBoxOutService
    {
        private readonly CAEDB01Context _context;
        private readonly MachineBoxingManagement.Services.BoxOutService _winformBoxoutService;
        private readonly IConfiguration _configuration;

        public BoxOutService(CAEDB01Context context, IConfiguration config)
        {
            _context = context;
            _configuration = config;
            _winformBoxoutService = new MachineBoxingManagement.Services.BoxOutService(_configuration.GetValue<string>("ConnectionStrings:CAEDB01Connection"), _configuration.GetValue<string>("ConnectionStrings:CAEServiceConnection"));
        }

        public InstockInfo GetInstockTime(string partNumber, out string errMsg) 
        {
            errMsg = string.Empty;

            InstockInfo InstockInfo = null;

            Tuple<string, string, string> _instockInfo = null;
            
            try
            {
                _instockInfo = _winformBoxoutService.GetInstockTime(new List<string> { partNumber }, out string instockErr).FirstOrDefault();
                errMsg = instockErr;
            }
            catch (Exception)
            {

            }

            if (string.IsNullOrEmpty(errMsg) && _instockInfo != null)
            {
                InstockInfo = new InstockInfo()
                {
                    PN = _instockInfo.Item1,
                    CDT = _instockInfo.Item2,
                    SPEC_NOTE = _instockInfo.Item3
                };
            }

            return InstockInfo;
        }

        /// <summary>
        /// 取出維護[查詢資料]
        /// </summary>
        /// <param name="partNumber"></param>
        /// <param name="model"></param>
        /// <param name="locations"></param>
        /// <param name="options"></param>
        /// <param name="styles"></param>
        /// <param name="statuses"></param>
        /// <param name="bufferArea"></param>
        /// <returns></returns>
        public async Task<Tuple<List<PartNumber_Model_Desc>, string>> QueryMachines(QueryRule conditions, int[]? favorites)
        {
            var Res = new List<PartNumber_Model_Desc>();
            var ResWcf = _winformBoxoutService.QueryMachines(conditions, out var errorMsg);
            var StatusMappingTable = _context.BoxingStatus.ToList();
            foreach (var item in ResWcf)
            {
                PartNumber_Model_Desc PnDesc = new PartNumber_Model_Desc()
                {  
                    ID = item.ID,//db pkey，當作取出流程的主鍵
                    serial_No = item.ID,//for excel id column display
                    Part_Number = item.PartNumber,
                    Is_Buffer_Area = item.BufferArea,
                    Desc = item.Description,
                    Model = item.Model,
                    InsDate = DateTime.TryParse(item.InStockDate, out var dt) ? Convert.ToDateTime(item.InStockDate).ToString("yyyy-MM-dd HH:mm:ss") : dt.ToString("yyyy-MM-dd HH:mm:ss"),
                    SSN = item.SSN,
                    Boxing_Location_Cn = item.Location,
                    Boxing_Option_Cn = item.BoxingOption, 
                    Boxing_Series = item.BoxingName,
                    Boxing_Serial = item.BoxingSerial,
                    Turtle_Level = item.StackLevel.HasValue ? item.StackLevel.Value : 0,
                    Status_Id = StatusMappingTable.Where(c => c.Name == item.Status).FirstOrDefault()?.Id ?? -1,
                    Operator = item.Operator,
                    OperateTime = item.OperationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    TakerOut_Operator = string.IsNullOrEmpty(item.Takeouter) ? "" : item.Takeouter,
                    TakeOut_OperateTime = item.TakeoutTime.HasValue ? item.TakeoutTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                    Is_Favorite = favorites?.Contains(item.ID) ?? false
                };
                Res.Add(PnDesc);
            }

            return new Tuple<List<PartNumber_Model_Desc>, string>(Res, errorMsg);
        }

        /// <summary>
        /// 取出維護[機台取出]
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<Tuple<List<int>, string>> TakeOutMachines(string userName, int[] ids)
        {       
            //取得取出成功的機台ID
            var outResult = _winformBoxoutService.TakeoutMachines(ids?.ToList(), userName, out string errorMsg);
            return new Tuple<List<int>, string>(outResult, errorMsg);
        }

        /// <summary>
        /// 取出維護[更新暫存區]
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<Tuple<List<int>, string>> SaveMachineBufferArea(string userName, Dictionary<int, bool> ids)
        {
            //取得取出更新暫存區的機台ID
            var outResult = _winformBoxoutService.SaveMachineBufferArea(ids, userName, out string errorMsg);
            return new Tuple<List<int>, string>(outResult, errorMsg);
        }
    }
}
