using System.Threading.Tasks;
using MachineBoxingManagement.Repositories.Models;

using Microsoft.AspNetCore.Mvc;
using MachineBoxingManagement.Web.Models.Dto;
using System.Collections.Generic;

namespace MachineBoxingManagement.Web.Services.Interfaces
{
    public interface IXlsxReportService
    {
        /// <summary>
        /// (db資料+暫存資料)匯出外箱貼紙excel檔
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<FileStreamResult> Export_Sticker(List<PartNumber_Model_Desc> data);

        /// <summary>
        /// (裝箱暫存資料)匯出excel清單
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public Task<FileStreamResult> Export_Temp_Data(List<PartNumber_Model_Desc> datas);

        /// <summary>
        /// (暫存)匯出excel清單
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public Task<FileStreamResult> Export_Favorite_Data(List<Favorite_PartNumber_Model_Desc> datas);
    }


}
