using System;
using System.Collections.Generic;
using System.Text;
using MachineBoxingManagement.Repositories.Models;
using MachineBoxingManagement.Services.Models;
using UniversalLibrary.Models;

namespace MachineBoxingManagement.Services.Interfaces
{
    public interface IBoxInService
    {
        Result<List<PnXStatus>> GetPnXStatus(List<string> partnumbers);

        Result<List<Tuple<string, string>>> GetPnOaCategory(List<string> partnumbers);

        Result<List<Tuple<string, string, string, DateTime?>>> GetPnModelDesc(List<string> partnumbers);

        Result<string> GetBoxingName(string partnumber, string model, string BoxingOption, string BoxingStyle, DateTime instockTime);

        Result<List<MachineBoxingInfo>> GetDBBoxes(List<MachineUniBoxingProp> boxes);

        Result<Tuple<int, int, int>> GetBoxingSerialInfo(string boxingName, int boxingLocationId, int? specifyBoxSerial = null);

        Result<List<BoxingLocation>> EnumBoxingLocations();

        Result<List<BoxingOption>> EnumBoxingOptions();

        Result<List<BoxingStyle>> EnumBoxingStyles();

        Result<int> SaveBoxingInfos(List<MachineBoxingInfo> infos);

    }
}
