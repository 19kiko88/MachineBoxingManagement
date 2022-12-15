using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using MachineBoxingManagement.Web.Services.Implements;
using MachineBoxingManagement.Web.Services.Interfaces;
using UniversalLibrary.Models;
using System.Threading.Tasks;
using MachineBoxingManagement.Web.Models.BoxIn;
using MachineBoxingManagement.Web.Models.Dto;
using MachineBoxingManagement.Web.Services.Common.Extensions;
using System.Collections.Generic;

namespace MachineBoxingManagement.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BoxInController : ControllerBase
    {
        private readonly IBoxInService _boxInService;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //private ISession _session => _httpContextAccessor.HttpContext.Session;


        public BoxInController(IBoxInService boxInService, IHttpContextAccessor httpContextAccessor)
        {
            _boxInService = boxInService;
            //_httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// P/N輸入檢核 & Input更新資料取得
        /// </summary>
        /// <returns></returns>
        //[HttpGet("{saveTemp}/{partNumber}/{location}/{machineOption}/{machineStyle}/{ssn?}")]
        [HttpPost]        
        public async Task<Result<Box_In_Processing>> ProcessingPN([FromBody] Box_In_Process_Input_Parameters data)
        {           
            var result = new Result<Box_In_Processing>() { Success = false };
            data.PartNumber = data.PartNumber.Replace("\t", "");//去除條碼輸入有時候產生的特殊字元(Tab)
           
            try
            {
                var Res = new Box_In_Processing();
                var Tuple_PN_Data = await _boxInService.ProcessingPN(data.UserName, data.PartNumber, data.SSN, data.Location, data.MachineOption, data.MachineStyle);
                var PN_Data = Tuple_PN_Data.Item1;
                var errorMsg = Tuple_PN_Data.Item2;

                if (!string.IsNullOrEmpty(errorMsg))
                {//未通過檢核
                    result.Message = errorMsg;
                }
                else
                {//通過檢核

                    //Setting Stocking Data                    
                    var Tuple_Stocking_Info = await _boxInService.Get_Machine_In_Box_Info(PN_Data.Boxing_Series, data.Location, data.SaveTemp, data.PnDatas);
                    var Stocking_Info = Tuple_Stocking_Info.Item1;
                    errorMsg = Tuple_Stocking_Info.Item2;

                    if (!string.IsNullOrEmpty(errorMsg))
                    {//儲位資訊取得失敗
                        result.Message = errorMsg;
                        return result;
                    }
                    else
                    {//儲位資訊取得成功
                        PN_Data.Boxing_Serial = Stocking_Info.Box_Serial;
                        PN_Data.Turtle_Level = Stocking_Info.Turtle_Level;

                        Res.PartNumber_Model_Desc = PN_Data;
                        Res.Stocking_Info = Stocking_Info;
                    }
                }

                result.Content = Res;
                result.Success = true;                
            }
            catch (System.Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 裝箱維護計算新箱號
        /// </summary>
        /// <param name="boxingSeries"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("{boxingSeries}")]
        public async Task<Result<Stocking_Info>> Get_Stocking_Info(string boxingSeries, [FromBody] Box_In_Process_Input_Parameters data)
        {            
            var errorMsg = string.Empty;
            var result = new Result<Stocking_Info>() { Success = false };

            try
            {
                var Tuple_Res = await _boxInService.Get_Machine_In_Box_Info(boxingSeries, data.Location, data.SaveTemp, data.PnDatas);                
                errorMsg = Tuple_Res.Item2;

                if (!string.IsNullOrEmpty(errorMsg))
                {//儲位資訊取得失敗
                    result.Message = errorMsg;
                    return result;
                }

                result.Content = Tuple_Res.Item1;
                result.Success = true;
            }
            catch (System.Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 變更箱號時，取得即時庫存資訊
        /// </summary>
        /// <param name="boxingSeries"></param>
        /// <param name="location"></param>
        /// <param name="boxSerial"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Result<Stocking_Info>> Get_Machine_In_Box_Info_By_BoxSerial([FromBody]Box_In_Process_Input_Parameters data)
        {
            var result = new Result<Stocking_Info>() { Success = false };

            try
            {
                var Tuple_Res = await _boxInService.Get_Machine_In_Box_Info_By_BoxSerial(data.BoxSeries, data.Location, data.BoxSerial, data.PnDatas);
                var errorMsg = Tuple_Res.Item2;

                if (!string.IsNullOrEmpty(errorMsg))
                {//儲位資訊取得失敗
                    result.Message = errorMsg;
                    return result;
                }

                result.Content = Tuple_Res.Item1;
                result.Success = true;
            }
            catch (System.Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 裝箱維護-批次儲存
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Result<int>> SaveBoxingInfos([FromBody] List<PartNumber_Model_Desc> datas)
        {
            var errorMsg = string.Empty;
            var result = new Result<int>() { Content = 0, Success = false };

            try
            {
                result.Content = await _boxInService.SaveBoxingInfos(datas);

                if (!string.IsNullOrEmpty(errorMsg))
                {
                    result.Message = errorMsg;
                    return result;
                }

                result.Success = true;
            }
            catch (System.Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
