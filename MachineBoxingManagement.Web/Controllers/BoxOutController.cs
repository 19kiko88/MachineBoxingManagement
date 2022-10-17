using MachineBoxingManagement.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UniversalLibrary.Models;
using MachineBoxingManagement.Web.Models.Dto;
using MachineBoxingManagement.Web.Models.BoxOut;
using System.Collections.Generic;
using System.Linq;
using System;



namespace MachineBoxingManagement.Web.Controllers
{

    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BoxOutController : ControllerBase
    {
        private readonly IBoxOutService _boxOutService;

        public BoxOutController(IBoxOutService boxOutService)
        {
            _boxOutService = boxOutService;
        }

        /// <summary>
        /// 取出維護-機台查詢
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<Result<List<PartNumber_Model_Desc>>> QueryMachines(Object_QueryMachines data)
        {
            var errorMsg = string.Empty;
            var result = new Result<List<PartNumber_Model_Desc>>() { Success = false };
            DateTime sd = Convert.ToDateTime("0001/01/01 00:00:00");
            DateTime ed = Convert.ToDateTime("9999/12/31 23:59:59");

            try
            {
                var buffer_areaCount = 0;
                data.buffer_areas?.ToList().ForEach(c => buffer_areaCount += c);

                var conditions = new MachineBoxingManagement.Services.Models.QueryRule()
                {
                    PartNumber = data.pn,
                    Model = data.model,
                    Sd_BoxIn = data.take_in_dt_s,
                    Ed_BoxIn = data.take_in_dt_e,
                    Sd_BoxOut = data.take_out_dt_s,
                    Ed_BoxOut = data.take_out_dt_e,
                    LocationIds = data.locations.ToList(),
                    OptionIds = data.options.ToList(),
                    StyleIds = data.styles.ToList(),
                    Statuses = data.statuses.ToList(),
                    BufferAreas = buffer_areaCount//轉換暫存區查詢條件：0 =>全部選, 1 => 非暫存區, 2=>暫存區, 3=>全選
                };
                var TupleRes = await _boxOutService.QueryMachines(conditions, data.favorites);
                result.Content = TupleRes.Item1;


                if (!string.IsNullOrEmpty(TupleRes.Item2))
                {
                    result.Message = TupleRes.Item2;

                }
                result.Success = true;
            }
            catch (System.Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 取出維護[機台取出]
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Result<List<int>>> TakeOutMachines([FromBody] Object_TakeOutMachines datas)
        {
            var result = new Result<List<int>>() { Success = false };
            try
            {
                var ResTuple = await _boxOutService.TakeOutMachines(datas.userName, datas.ids);
                result.Content = ResTuple.Item1;
                result.Message = ResTuple.Item2;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }

            return result;
        }

        /// <summary>
        /// 取出維護[機台取出]
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Result<List<int>>> SaveMachineBufferArea([FromBody] Object_TakeOutMachines data)
        {
            var result = new Result<List<int>>() { Success = false };
            try
            {
                var DicBoxOut = new Dictionary<int, bool>();
                data.BoxOut_Item.ForEach(c =>
                {
                    DicBoxOut.Add(c.Id, c.IsBufferArea);
                });

                var ResTuple = await _boxOutService.SaveMachineBufferArea(data.userName, DicBoxOut);
                result.Content = ResTuple.Item1;
                result.Message = ResTuple.Item2;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }

            return result;
        }
    }
}
