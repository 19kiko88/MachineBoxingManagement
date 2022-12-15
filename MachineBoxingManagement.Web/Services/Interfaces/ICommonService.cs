using System.Threading.Tasks;
using MachineBoxingManagement.Repositories.Models;
using CAEService;
using System.Collections.Generic;

namespace MachineBoxingManagement.Web.Services.Interfaces
{
    public interface ICommonService
    {
        /// <summary>
        /// 庫房(BoxingLocation)
        /// </summary>
        /// <returns></returns>
        Task<BoxingLocation[]> GetBoxingLocations();

        /// <summary>
        /// 機台選項(BoxingOption)
        /// </summary>
        /// <returns></returns>
        Task<BoxingOption[]> GetBoxingOptions();

        /// <summary>
        /// 樣式選項(BoxingStyle)
        /// </summary>
        /// <returns></returns>
        Task<BoxingStyle[]> GetBoxingStyle();

        /// <summary>
        /// 機台狀態選項(BoxingStatus)
        /// </summary>
        /// <returns></returns>
        Task<BoxingStatus[]> GetBoxingStatus();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deptname"></param>
        /// <returns></returns>
        public List<Employee> GetDeptEmployees(string deptname);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public Employee GetEmployeeInfo(string username);
    }


}
