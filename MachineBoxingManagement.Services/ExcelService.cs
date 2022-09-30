using MachineBoxingManagement.Repositories.Models;
using MachineBoxingManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using MachineBoxingManagement.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using UniversalLibrary.Models;
using MachineBoxingManagement.Services.Models;
using Excel_Manipulate;

namespace MachineBoxingManagement.Services
{
    public class ExcelService : IExcelService
    {
        private string _stickerTemplateFile;
        private string _blankTemplateFile;

        private readonly CAEDB01Context _caEDB01Context;
        private readonly DbContextOptionsBuilder<CAEDB01Context> _dbContextOptionsBuilder;
        private readonly BoxInService _boxInService;

        public ExcelService(string dbconn, BoxInService boxInService)
        {
            _stickerTemplateFile = Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"Content\外箱紙範本.xlsx");
            _blankTemplateFile = Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"Content\template.xlsx");
            _dbContextOptionsBuilder = new DbContextOptionsBuilder<CAEDB01Context>();
            _dbContextOptionsBuilder.UseNpgsql(dbconn);
            _caEDB01Context = new CAEDB01Context(_dbContextOptionsBuilder.Options);
            _boxInService = boxInService;
        }

        public Result<XLWorkbook> GenerateBoxingStickers(List<MachineBoxingInfo> data, List<Tuple<string, string, string, DateTime?>> pnModelDescs = null)
        {
            ////test
            ////data = _caEDB01Context.MachineBoxingInfo.ToList();
            var result = new Result<XLWorkbook>
            {
                Success = true,
            };

            try
            {
                //model
                if (pnModelDescs == null)
                {
                    pnModelDescs = new List<Tuple<string, string, string, DateTime?>>();
                }

                var pndescresult = _boxInService.GetPnModelDesc(data.Where(a => !pnModelDescs.Select(b => b.Item1.ToUpper()).Contains(a.PartNumber.ToUpper())).Select(a => a.PartNumber).Distinct().ToList());
                if (pndescresult.Success)
                {
                    pnModelDescs.AddRange(pndescresult.Content);
                }
                else
                {
                    //無法取得model
                }

                //oaspec
                var pnoaresult = _boxInService.GetPnOaCategory(data.Select(a => a.PartNumber).Distinct().ToList());
                if (!pnoaresult.Success)
                {
                    //無法取得oaspec
                }


                var newxlswb = new XLWorkbook(_blankTemplateFile);

                var workbook = new XLWorkbook(_stickerTemplateFile);
                Dictionary<string, IXLRange> xlRanges = new Dictionary<string, IXLRange>();
                xlRanges.Add("預設", workbook.Worksheet("NPI").Range("A1", "E29"));
                xlRanges.Add("待燒OA", workbook.Worksheet("待燒OA").Range("A1", "F29"));
                xlRanges.Add("已燒OA", workbook.Worksheet("已燒OA").Range("A1", "F29"));
                xlRanges.Add("海運待燒OA", workbook.Worksheet("海運待燒").Range("A1", "F29"));
                xlRanges.Add("ORT", workbook.Worksheet("ORT").Range("A1", "E29"));
                xlRanges.Add("永久保留", workbook.Worksheet("永久保留").Range("A1", "E29"));


                var allLocations = _caEDB01Context.BoxingLocation.Select(a => a).ToList();
                var allBoxingOptions = _caEDB01Context.BoxingOption.Select(a => a).ToList();
                var allStatus = _caEDB01Context.BoxingStatus.Select(a => a).ToList();
                var allStyleNames = _caEDB01Context.BoxingStyle.Select(a => a.Name).ToList();
                var npiProp = new NpiStickerProp();
                var ortProp = new NpiStickerProp
                {
                    SPartNumber = new PropCoord { Y = 7, X = 0 },
                    SModel = new PropCoord { Y = 7, X = 1 },
                    SQuanty = new PropCoord { Y = 7, X = 2 },
                    STakeoutDate = new PropCoord { Y = 7, X = 3 }
                };
                var oaProp = new OaStickerProp
                {
                    SPartNumber = new PropCoord { Y = 8, X = 0 },
                    SModel = new PropCoord { Y = 8, X = 1 },
                    SQuanty = new PropCoord { Y = 8, X = 3 },
                    Colspan = 6,
                    RowHeight = 27
                };
                foreach (var dataopt in data.GroupBy(a => a.BoxingLocationId.ToString() + "." + a.BoxingName))
                {
                    var location = allLocations.Where(a => a.Id == Convert.ToInt32(dataopt.Key.Split('.').GetValue(0).ToString())).Select(a => a.Name).FirstOrDefault();
                    var sheetName = location + "-" + dataopt.Key.Split('.').GetValue(1).ToString();

                    //var curWs = (dataopt.Key.Contains("OA") ? newxlswb.Worksheet("外箱貼紙OA").CopyTo(sheetName) : newxlswb.Worksheet("外箱貼紙").CopyTo(sheetName));
                    var sourceWs = (dataopt.Key.Contains("OA") ? newxlswb.Worksheet("外箱貼紙OA") : newxlswb.Worksheet("外箱貼紙"));
                    var curWs = newxlswb.AddWorksheet(sheetName);

                    for (int i = 1; i <= oaProp.Colspan * oaProp.Rowcapacity; i++)
                    {
                        curWs.Column(i).Width = sourceWs.Column(i).Width;
                    }

                    

                    //normal
                    var currow = 1;
                    var curcol = 1;

                    var currcap = 1;

                    IXLRange xlrng;
                    dynamic boxProp = npiProp;
                    switch (dataopt.Key.Split('.').GetValue(1).ToString())
                    {
                        case "永久保留":
                            xlrng = xlRanges["永久保留"];
                            boxProp = ortProp;
                            break;
                        case "ORT":
                            xlrng = xlRanges["ORT"];
                            boxProp = ortProp;
                            break;
                        case "待燒OA":
                            xlrng = xlRanges["待燒OA"];
                            boxProp = oaProp;
                            break;
                        case "已燒OA":
                            xlrng = xlRanges["已燒OA"];
                            boxProp = oaProp;
                            break;
                        case "海運待燒OA":
                            xlrng = xlRanges["海運待燒OA"];
                            boxProp = oaProp;
                            break;
                        default:
                            xlrng = xlRanges["預設"];
                            boxProp = npiProp;
                            break;
                    };

                    //initialize
                    curWs.RowHeight = (double)boxProp.RowHeight;
                    curWs.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                    curWs.PageSetup.PaperSize = XLPaperSize.A4Paper;
                    curWs.PageSetup.FitToPages(1, 0);
                    curWs.PageSetup.AddVerticalPageBreak((int)boxProp.Colspan * (int)boxProp.Rowcapacity);
                    curWs.SheetView.SetView(XLSheetViewOptions.PageBreakPreview);
                    curWs.SheetView.ZoomScale = 50;

                    var pageCnt = 1;

                    foreach (var box in dataopt.GroupBy(a => a.BoxingSerial))
                    {
                        IXLCell ccell;
                        IXLWorksheet rsheet;
                        var rrow = 0;
                        var rcol = 0;

                        ccell = curWs.Cell(currow, curcol);
                        rrow = currow;
                        rcol = curcol;
                        rsheet = curWs;

                        //initialize
                        rsheet.Cell(1, (int)boxProp.Colspan * (int)boxProp.Rowcapacity + (int)boxProp.DefaultOffsetCol).Value = " ";

                        xlrng.CopyTo(ccell);

                        //rowheight
                        for (int i = currow; i < currow + (int)boxProp.Rowspan; i++)
                        {
                            rsheet.Row(i).Height = curWs.RowHeight;
                        }

                        var datasrow = (int)boxProp.SPartNumber.Y;

                        //basic
                        //rsheet.Cell(rrow, rcol).Style.Font.FontSize = 20.0;
                        rsheet.Cell(rrow, rcol).Value = box.FirstOrDefault().BoxingName;
                        rsheet.Cell(rrow + (int)boxProp.OpDate.Y, rcol + (int)boxProp.OpDate.X).Value = box.FirstOrDefault().OperateTime.ToString("yyyy/MM/dd");
                        rsheet.Cell(rrow + (int)boxProp.BoxSerial.Y, rcol + (int)boxProp.BoxSerial.X).Value = box.FirstOrDefault().BoxingSerial;
                        rsheet.Cell(rrow + (int)boxProp.Location.Y, rcol + (int)boxProp.Location.X).Value = location;
                        rsheet.Cell(rrow + (int)boxProp.Operator.Y, rcol + (int)boxProp.Operator.X).Value = box.FirstOrDefault().Operator;

                        //machine
                        var machCnt = 0;
                        foreach (var mach in box)
                        {
                            //pn
                            rsheet.Cell(rrow + machCnt + datasrow, rcol + (int)boxProp.SPartNumber.X).Value = mach.PartNumber;
                            //model
                            rsheet.Cell(rrow + machCnt + datasrow, rcol + (int)boxProp.SModel.X).Value = pnModelDescs == null ? "NA" :
                                pnModelDescs.Where(a => a.Item1.ToUpper() == mach.PartNumber.ToUpper()).Select(a => a.Item3).FirstOrDefault() ?? "N/A";
                            //quantity
                            rsheet.Cell(rrow + machCnt + datasrow, rcol + (int)boxProp.SQuanty.X).Value = 1;

                            if (boxProp is OaStickerProp)
                            {
                                //oa category
                                rsheet.Cell(rrow + machCnt + datasrow, rcol + (int)boxProp.SCategory.X).Value = pnoaresult.Content.Any(a => a.Item1 == mach.PartNumber) ?
                                    (pnoaresult.Content.Any(a => a.Item1 == mach.PartNumber && a.Item2.Length == 2) ?
                                    pnoaresult.Content.Where(a => a.Item1 == mach.PartNumber && a.Item2.Length == 2).Select(a => a.Item2).First() :
                                    "X") :
                                    "";
                                //instock time
                                rsheet.Cell(rrow + machCnt + datasrow, rcol + (int)boxProp.InStockDate.X).Value = pnModelDescs == null ? "NA" :
                                    pnModelDescs.Where(a => a.Item1.ToUpper() == mach.PartNumber.ToUpper()).Select(a => a.Item4).FirstOrDefault() == null ? "N/A"
                                    : "'" + pnModelDescs.Where(a => a.Item1.ToUpper() == mach.PartNumber.ToUpper()).Select(a => a.Item4).FirstOrDefault().Value.ToString("MM/yy");
                            }

                            machCnt++;
                        }

                        //subtotal
                        rsheet.Cell(rrow + (int)boxProp.Subtotal.Y, rcol + (int)boxProp.Subtotal.X).Value = box.Count();


                        if (++currcap > boxProp.Rowcapacity)
                        {
                            curcol = 1;
                            currcap = 1;
                            currow += (int)boxProp.Rowspan;
                            for (int i = currow; i < currow + (int)boxProp.Rowspan; i++)
                            {
                                rsheet.Row(i).Height = curWs.RowHeight;
                            }
                            curWs.PageSetup.AddHorizontalPageBreak((int)boxProp.Rowspan * pageCnt++);
                        }
                        else
                        {
                            curcol += (int)boxProp.Colspan;
                        }

                    }


                    curWs.PageSetup.PrintAreas.Add(1, 1, (int)boxProp.Rowspan * pageCnt, (int)boxProp.Colspan * (int)boxProp.Rowcapacity);
                    

                }
                newxlswb.Worksheet("外箱貼紙OA").Delete();
                newxlswb.Worksheet("外箱貼紙").Delete();
                result.Content = newxlswb;
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Exception = err;
                result.Message = err.Message;
            }

            

            return result;
        }

        public Result<byte[]> ExportExcelReport<T>(List<T> data)
        {
            var result = new Result<byte[]>
            {
                Success = true,
            };

            try
            {
                CreateExcel excel = new CreateExcel();
                CreateExcel.SheetInfo sheet = new CreateExcel.SheetInfo
                {
                    SheetName = "Report",
                    FreezeSheet = new CreateExcel.SheetInfo.SheetViewFreeze
                    {
                        RowNumber = 1
                    },
                    Protect = false
                };
                
                var dt = Extentions.ConvertToDataTable<T>(data);
                sheet.importData(dt, true, out string importErr);
                if (!string.IsNullOrEmpty(importErr))
                {
                    result.Success = false;
                    result.Message = $"Import Data Error: {importErr}";
                }
                else
                {
                    excel.Sheets.Add(sheet);
                    result.Content = excel.saveToByte(out string saveErr);
                    if (!string.IsNullOrEmpty(importErr))
                    {
                        result.Success = false;
                        result.Message = $"save Data Error: {saveErr}";
                    }

                }
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Exception = err;
                result.Message = err.Message;
            }

            return result;

        }


    }
}
