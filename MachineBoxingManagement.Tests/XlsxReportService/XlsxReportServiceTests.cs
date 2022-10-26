using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using MachineBoxingManagement.Repositories.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MachineBoxingManagement.Web.Models.Dto;
using MachineBoxingManagement.Repositories.Models;

namespace MachineBoxingManagement.Tests.XlsxReportService
{
    [TestClass]
    public class XlsxReportServiceTests
    {
        private IConfiguration _configuration;
        private MachineBoxingManagement.Web.Services.Implements.XlsxReportService _xlsxReportService;
        private Mock<CAEDB01Context> _mockContext = new Mock<CAEDB01Context>();

        public XlsxReportServiceTests() 
        {
            _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .Build();

            //var dbContextOptions = new DbContextOptionsBuilder<CAEDB01Context>()
            //           .UseNpgsql(_configuration.GetValue<string>("ConnectionStrings:CAEDB01Connection"))
            //           .Options;
            //var dbContext = new CAEDB01Context(dbContextOptions);
        }

        /// <summary>
        /// [裝箱維護]批次儲存前先列印出裝箱貼紙Excel
        /// </summary>
        [TestMethod()]
        public void ExportSticker_Get_FileStream()
        {
            var ListPnData = new List<PartNumber_Model_Desc> {
                    new PartNumber_Model_Desc
                    {
                        Part_Number = "90NB0S51-T00070",
                        Boxing_Location_Id = 1,
                        Boxing_Option_Id = 1,
                        Boxing_Style_Id = 1,
                        Boxing_Series = "待燒OA",
                        Boxing_Serial = 1,
                        Turtle_Level = 1,
                        Status_Id = 666,
                        Operator = "Homer_Chen",
                        OperateTime = DateTime.Now.ToString()
                    }
                };

            var InfoData = new List<MachineBoxingInfo> {
                    new MachineBoxingInfo{
                        PartNumber = ListPnData[0].Part_Number,
                        BoxingLocationId = ListPnData[0].Boxing_Location_Id,
                        BoxingOptionId = ListPnData[0].Boxing_Option_Id,
                        BoxingStyleId = ListPnData[0].Boxing_Style_Id,
                        BoxingName = ListPnData[0].Boxing_Series,
                        BoxingSerial = ListPnData[0].Boxing_Serial,
                        StackLevel = ListPnData[0].Turtle_Level,
                        StatusId = ListPnData[0].Status_Id,
                        Operator = ListPnData[0].Operator,
                        OperateTime = Convert.ToDateTime(ListPnData[0].OperateTime)
                    }
                }.AsQueryable();

            /*
             * Mock EF DbSet
             *Ref：https://learn.microsoft.com/zh-tw/ef/ef6/fundamentals/testing/mocking?redirectedfrom=MSDN
             */
            var mockSet_MachineBoxingInfo = new Mock<DbSet<MachineBoxingInfo>>();
            mockSet_MachineBoxingInfo.As<IQueryable<MachineBoxingInfo>>().Setup(m => m.Provider).Returns(InfoData.Provider);
            mockSet_MachineBoxingInfo.As<IQueryable<MachineBoxingInfo>>().Setup(m => m.Expression).Returns(InfoData.Expression);
            mockSet_MachineBoxingInfo.As<IQueryable<MachineBoxingInfo>>().Setup(m => m.ElementType).Returns(InfoData.ElementType);
            mockSet_MachineBoxingInfo.As<IQueryable<MachineBoxingInfo>>().Setup(m => m.GetEnumerator()).Returns(InfoData.GetEnumerator());
            _mockContext.Setup(q => q.MachineBoxingInfo).Returns(mockSet_MachineBoxingInfo.Object);

            //驗證產生的MockSet內容
            var TestData = _mockContext.Object.MachineBoxingInfo.ToList();
            Assert.AreEqual(1, TestData?.Count);
            Assert.AreEqual("90NB0S51-T00070", TestData[0]?.PartNumber);

            var _xlsxReportService = new MachineBoxingManagement.Web.Services.Implements.XlsxReportService(_mockContext.Object, _configuration);
            try
            {
                var Res = _xlsxReportService.Export_Sticker(ListPnData).Result;
            }
            catch (Exception Ex)
            {
                Assert.Fail(Ex.Message);
            }
        }

        /// <summary>
        /// [裝箱維護]列印機台暫存清單Excel
        /// </summary>
        [TestMethod()]
        public void ExportTempData_Get_FileStream()
        {
            var ListPnData = new List<PartNumber_Model_Desc> {
                    new PartNumber_Model_Desc
                    {
                        Part_Number = "90NB0S51-T00070",
                        Boxing_Location_Id = 1,
                        Boxing_Option_Id = 1,
                        Boxing_Style_Id = 1,
                        Boxing_Series = "待燒OA",
                        Boxing_Serial = 1,
                        Turtle_Level = 1,
                        Status_Id = 666,
                        Operator = "Homer_Chen",
                        OperateTime = DateTime.Now.ToString()
                    }
                };

            var _xlsxReportService = new MachineBoxingManagement.Web.Services.Implements.XlsxReportService(_mockContext.Object, _configuration);
            try
            {
                _xlsxReportService.Export_Temp_Data(ListPnData);
            }
            catch (Exception Ex)
            {
                Assert.Fail(Ex.Message);
            }
        }

        /// <summary>
        /// [取出維護]暫存機台清單Excel
        /// </summary>
        [TestMethod()]
        public void ExportFavoriteData_Get_FileStream()
        {
            var ListFavoritePnData = new List<Favorite_PartNumber_Model_Desc> {
                    new Favorite_PartNumber_Model_Desc
                    {
                        ID = 806,
                        Part_Number = "90NB0S51-T00070",
                        Boxing_Series = "待燒OA",
                        Boxing_Serial = 1,
                        Turtle_Level = 1,
                    }
                };

            var _xlsxReportService = new MachineBoxingManagement.Web.Services.Implements.XlsxReportService(_mockContext.Object, _configuration);
            try
            {
                _xlsxReportService.Export_Favorite_Data(ListFavoritePnData);
            }
            catch (Exception Ex)
            {
                Assert.Fail(Ex.Message);
            }
        }
    }
}
