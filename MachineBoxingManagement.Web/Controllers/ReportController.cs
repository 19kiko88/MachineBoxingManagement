using Microsoft.AspNetCore.Mvc;
using MachineBoxingManagement.Services;
using MachineBoxingManagement.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using MachineBoxingManagement.Web.Services.Interfaces;
using System.Threading.Tasks;
using MachineBoxingManagement.Web.Models.Dto;
using System.Collections.Generic;

namespace MachineBoxingManagement.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IXlsxReportService _xlsReportService;

        public ReportController(IXlsxReportService xlsReportService)
        {
            _xlsReportService = xlsReportService;
        }

        /// <summary>
        /// (db資料+暫存資料)匯出外箱貼紙excel檔
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> Export_Sticker([FromBody] List<PartNumber_Model_Desc> data)
        {           
            return await _xlsReportService.Export_Sticker(data);
        }

        /// <summary>
        /// (裝箱暫存資料)匯出excel清單
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> Export_Temp_Data([FromBody] List<PartNumber_Model_Desc> data)
        {
            return await _xlsReportService.Export_Temp_Data(data);
        }

        /// <summary>
        /// (暫存)匯出excel清單
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> Export_Favorite_Data([FromBody] List<Favorite_PartNumber_Model_Desc> data)
        {
            return await _xlsReportService.Export_Favorite_Data(data);
        }
    }
}
