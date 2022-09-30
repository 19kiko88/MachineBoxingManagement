using MachineBoxingManagement.Repositories.Models;
using MachineBoxingManagement.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineBoxingManagement.Services.Interfaces
{
    public interface IBoxOutService
    {
        List<MachineBoxingInfoView> QueryMachines(QueryRule rule, out string errMsg, List<MachineBoxingInfo> sources = null);
        List<string> GetPartNumberFromModel(string model);
        List<Tuple<string, string, string>> GetInstockTime(List<string> pns, out string errMsg);
        List<int> TakeoutMachines(List<int> ids, string takeouter, out string errMsg);
        List<int> SaveMachineBufferArea(Dictionary<int, bool> ids, string operater, out string errMsg);
    }
}
