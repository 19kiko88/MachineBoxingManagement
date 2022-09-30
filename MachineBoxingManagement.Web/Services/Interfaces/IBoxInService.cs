using MachineBoxingManagement.Web.Models.BoxIn;
using MachineBoxingManagement.Web.Models.Dto;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MachineBoxingManagement.Web.Services.Interfaces
{
    public interface IBoxInService
    {
        /// <summary>
        /// 檢核輸入的PN
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="partNumber"></param>
        /// <param name="ssn"></param>
        /// <param name="location"></param>
        /// <param name="machineOption"></param>
        /// <param name="machineStyle"></param>
        /// <returns></returns>
        public Task<Tuple<PartNumber_Model_Desc, string>> ProcessingPN(string userName, string partNumber, string ssn, int location, int machineOption, int machineStyle);

        /// <summary>
        /// 取得裝箱系列資訊
        /// </summary>
        /// <param name="partNumber"></param>
        /// <param name="model"></param>
        /// <param name="machineOption"></param>
        /// <param name="machineStyle"></param>
        /// <param name="insDate"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public string GetBoxingName(string partNumber, string model, int machineOption, int machineStyle, DateTime insDate, out string errorMsg);

        /// <summary>
        /// 原Winform GetMIB()，取得裝箱儲位相關資訊(箱號(Serial), 箱內數量, 烏龜車層數)
        /// </summary>
        /// <param name="boxingSeries"></param>
        /// <param name="boxingLocation"></param>
        /// <param name="saveTemp"></param>
        /// <param name="tempDataList"></param>
        /// <returns></returns>
        public Task<Tuple<Stocking_Info, string>> Get_Machine_In_Box_Info(string boxingSeries, int boxingLocation, bool saveTemp = false, List<PartNumber_Model_Desc>? tempDataList = null);

        /// <summary>
        /// 取得指定箱號的裝箱儲位相關資訊(箱號(Serial), 箱內數量, 烏龜車層數)
        /// </summary>
        /// <param name="boxingSeries"></param>
        /// <param name="boxingLocation"></param>
        /// <param name="saveTemp"></param>
        /// <param name="tempDataList"></param>
        /// <returns></returns>
        public Task<Tuple<Stocking_Info, string>> Get_Machine_In_Box_Info_By_BoxSerial(string boxingSeries, int boxingLocation, int boxSerial, List<PartNumber_Model_Desc>? tempDataList = null);

        /// <summary>
        /// 儲存MachineBoxingInfo到DB
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<int> SaveBoxingInfos(List<PartNumber_Model_Desc> data);
    }
}
