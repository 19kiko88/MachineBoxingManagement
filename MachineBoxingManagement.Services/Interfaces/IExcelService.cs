using ClosedXML.Excel;
using MachineBoxingManagement.Repositories.Models;
using MachineBoxingManagement.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using UniversalLibrary.Models;

namespace MachineBoxingManagement.Services.Interfaces
{
    public interface IExcelService
    {
        Result<XLWorkbook> GenerateBoxingStickers(List<MachineBoxingInfo> data, List<Tuple<string, string, string, DateTime?>> pnModelDescs = null);
        Result<byte[]> ExportExcelReport<T>(List<T> data);
    }
}
