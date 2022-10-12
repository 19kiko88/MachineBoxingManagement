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
        [HttpGet]
        public async Task<Result<List<PartNumber_Model_Desc>>> QueryMachines(
            string? pn, string? model, string? s_takeInDt, string? e_takeInDt, string? s_takeOutDt, string? e_takeOutDt,
            [FromQuery(Name = "locations")] int[]? locations,
            [FromQuery(Name = "options")] int[]? options,
            [FromQuery(Name = "styles")] int[]? styles,
            [FromQuery(Name = "statuses")] int[]? statuses,
            [FromQuery(Name = "favorites")] int[]? favorites,
            [FromQuery(Name = "buffer_area")] int[]? buffer_area
            )
        {
            var errorMsg = string.Empty;
            var result = new Result<List<PartNumber_Model_Desc>>() { Success = false };
            DateTime sd = Convert.ToDateTime("0001/01/01 00:00:00");
            DateTime ed = Convert.ToDateTime("9999/12/31 23:59:59");

            try
            {
                var buffer_areaCount = 0;
                buffer_area?.ToList().ForEach(c => buffer_areaCount += c);

                var conditions = new MachineBoxingManagement.Services.Models.QueryRule()
                {
                    PartNumber = pn,
                    Model = model,
                    Sd_BoxIn = s_takeInDt,
                    Ed_BoxIn = e_takeInDt,
                    Sd_BoxOut = s_takeOutDt,
                    Ed_BoxOut = e_takeOutDt,
                    LocationIds = locations.ToList(),
                    OptionIds = options.ToList(),
                    StyleIds = styles.ToList(),
                    Statuses = statuses.ToList(),
                    BufferAreas = buffer_areaCount//轉換暫存區查詢條件：0 =>全部選, 1 => 非暫存區, 2=>暫存區, 3=>全選
                };
                var TupleRes = await _boxOutService.QueryMachines(conditions, favorites);
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
