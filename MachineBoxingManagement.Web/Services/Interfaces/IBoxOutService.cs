using MachineBoxingManagement.Web.Models.Dto;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MachineBoxingManagement.Web.Services.Interfaces
{
    public interface IBoxOutService
    {
        /// <summary>
        /// 重新包裝Winform BoxOutService.GetInstockTime為強型別
        /// </summary>
        /// <param name="partNumber"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public InstockInfo GetInstockTime(string partNumber, out string errMsg);

        /// <summary>
        /// 取出維護[查詢資料]
        /// </summary>
        /// <param name="partNumber"></param>
        /// <param name="model"></param>
        /// <param name="locations"></param>
        /// <param name="options"></param>
        /// <param name="styles"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public Task<Tuple<List<PartNumber_Model_Desc>, string>> QueryMachines(MachineBoxingManagement.Services.Models.QueryRule conditions, int[]? favorites);

        /// <summary>
        /// 取出維護[機台取出]
        /// </summary>
        /// <param name="userName">操作者</param>
        /// <param name="ids">要取出的機台id</param>
        /// <returns></returns>
        public Task<Tuple<List<int>, string>> TakeOutMachines(string userName, int[] ids);

        /// <summary>
        /// 取出維護[更新暫存區]
        /// </summary>
        /// <param name="userName">操作者</param>
        /// <param name="ids">要變更暫存區狀態的id</param>
        /// <returns></returns>
        public Task<Tuple<List<int>, string>> SaveMachineBufferArea(string userName, Dictionary<int, bool> ids);
    }
}
