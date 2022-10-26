using MachineBoxingManagement.Services;
using MachineBoxingManagement.Services.Interfaces;
using Microsoft.Extensions.Configuration;

using MachineBoxingManagement.Web.Services.Interfaces;
using System.IO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using MachineBoxingManagement.Web.Models.Dto;
using MachineBoxingManagement.Repositories.Data;
using System.Linq;
using MachineBoxingManagement.Repositories.Models;

namespace MachineBoxingManagement.Web.Services.Implements
{
    public class XlsxReportService: IXlsxReportService
    {
        private readonly CAEDB01Context _context;
        private const string DB_ConnectionString = "ConnectionStrings:CAEDB01Connection";
        private const string WCF_ConnectionString = "ConnectionStrings:CAEServiceConnection";
        private readonly IConfiguration _configuration;
        private readonly MachineBoxingManagement.Services.BoxInService _winformBoxinService;
        private readonly MachineBoxingManagement.Services.ExcelService _excelService;


        public XlsxReportService(CAEDB01Context context, IConfiguration config)
        {
            _context = context;
            _configuration = config;
            _winformBoxinService = new MachineBoxingManagement.Services.BoxInService(_configuration.GetValue<string>(DB_ConnectionString), _configuration.GetValue<string>(WCF_ConnectionString));
            _excelService = new ExcelService(_configuration.GetValue<string>(DB_ConnectionString), _winformBoxinService);
        }

        public async Task<FileStreamResult> Export_Sticker(List<PartNumber_Model_Desc> datas)
        {
            var Ms = new MemoryStream();

            var ListData = new List<MachineBoxingInfo>();
            var pnModelDescs = new List<Tuple<string, string, string, DateTime?>>();

            //原winform usedBoxes + machineBoxingInfos
            //machineBoxingInfos
            var GroupData = datas.GroupBy(g => new { g.Boxing_Location_Id, g.Boxing_Series, g.Boxing_Serial }).ToList();
            datas.ForEach(item =>
            {
                var Data = new MachineBoxingInfo()
                {
                    PartNumber = item.Part_Number,
                    Ssn = item.SSN,
                    BoxingLocationId = item.Boxing_Location_Id,
                    BoxingOptionId = item.Boxing_Option_Id,
                    BoxingStyleId = item.Boxing_Style_Id,
                    BoxingName = item.Boxing_Series,
                    BoxingSerial = item.Boxing_Serial,
                    StackLevel = item.Turtle_Level,
                    StatusId = item.Status_Id,
                    Operator = item.Operator,
                    OperateTime = Convert.ToDateTime(item.OperateTime)
                };
                ListData.Add(Data);
            });

            //usedBoxes
            var qq = _context.MachineBoxingInfo;
            GroupData.ForEach(item =>
            {
                var Data = _context.MachineBoxingInfo.Where(c => c.BoxingLocationId == item.Key.Boxing_Location_Id && c.BoxingName == item.Key.Boxing_Series && c.BoxingSerial == item.Key.Boxing_Serial).ToList();
                ListData.AddRange(Data);
            });
            

            //原winform 變數：pnModelDescs
            var data2 = datas.GroupBy(g => new { g.Part_Number, g.Desc, g.Model, g.ToOA, g.TearDown, g.MbResale, g.InsDate }).ToList();
            data2.ForEach(item =>
            {
                pnModelDescs.Add(new Tuple<string, string, string, DateTime?>(item.Key.Part_Number, item.Key.Desc, item.Key.Model, Convert.ToDateTime(item.Key.InsDate)));
            });


            using (var WorkBook = new XLWorkbook())
            {
                var XL_Workbook = _excelService.GenerateBoxingStickers(ListData, pnModelDescs)?.Content;
                XL_Workbook.SaveAs(Ms);
            }
            Ms.Seek(0, SeekOrigin.Begin);

            return new FileStreamResult(Ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "Report.xlsx"
            };
        }

        public async Task<FileStreamResult> Export_Temp_Data(List<PartNumber_Model_Desc> datas)
        {
            var ListData = new List<MachineBoxingManagement.Services.Models.MachineBoxingInfoView>();

            datas.ForEach(item =>
            {
                var Data = new MachineBoxingManagement.Services.Models.MachineBoxingInfoView()
                {
                    ID = item.serial_No,
                    PartNumber = item.Part_Number,
                    BufferArea = item.Is_Buffer_Area,
                    Description = item.Desc,
                    Model = item.Model,
                    SSN = item.SSN,
                    InStockDate = item.InsDate,
                    Location = item.Boxing_Location_Cn,
                    BoxingOption = item.Boxing_Option_Cn,
                    BoxingName = item.Boxing_Series,
                    BoxingSerial = item.Boxing_Serial,
                    StackLevel = item.Turtle_Level,
                    Status = item.Status_Id.ToString(),
                    Operator = item.Operator,
                    OperationTime = Convert.ToDateTime(item.OperateTime),
                    Takeout = item.Status_Id.ToString() == "777" ? true : false,
                    Takeouter = string.IsNullOrEmpty(item.TakerOut_Operator) ? "" : item.TakerOut_Operator,
                    TakeoutTime = DateTime.TryParse(item.TakeOut_OperateTime, out var tkop_time) ? tkop_time : null
                };
                ListData.Add(Data);
            });

            //排序
            ListData = ListData.OrderBy(c => c.Location).ThenBy(c => c.BoxingName).ThenBy(a => a.BoxingSerial).ToList();

            var excelBytes = _excelService.ExportExcelReport(ListData);
            MemoryStream Ms = new MemoryStream();
            Ms.Write(excelBytes.Content, 0, excelBytes.Content.Length);
            Ms.Seek(0, SeekOrigin.Begin);

            return new FileStreamResult(Ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "Report.xlsx"
            };
        }


        public async Task<FileStreamResult> Export_Favorite_Data(List<Favorite_PartNumber_Model_Desc> datas)
        {
            var ListData = datas.OrderBy(c => c.Boxing_Series).ThenBy(a => a.Boxing_Serial)
                .Select(item => new
                {
                    ID = item.ID,
                    PartNumber = item.Part_Number,
                    BoxingName = item.Boxing_Series,
                    BoxingSerial = item.Boxing_Serial,
                    StackLevel = item.Turtle_Level,
                    Model = item.Model,
                    Description = item.Desc,
                }).ToList();

            var excelBytes = _excelService.ExportExcelReport(ListData);
            MemoryStream Ms = new MemoryStream();
            Ms.Write(excelBytes.Content, 0, excelBytes.Content.Length);
            Ms.Seek(0, SeekOrigin.Begin);

            return new FileStreamResult(Ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "Report.xlsx"
            };
        }

    }
}
