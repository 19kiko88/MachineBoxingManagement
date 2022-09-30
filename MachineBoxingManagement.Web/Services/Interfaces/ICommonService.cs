using System.Threading.Tasks;
using MachineBoxingManagement.Repositories.Models;

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
    }


}
