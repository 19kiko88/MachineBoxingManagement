using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using Microsoft.EntityFrameworkCore;
using MachineBoxingManagement.Web.Services.Interfaces;
using MachineBoxingManagement.Web.Services.Implements;
using MachineBoxingManagement.Repositories.Data;
using MachineBoxingManagement.Web.Models.Dto;
using MachineBoxingManagement.Web.Models.BoxIn;
using Microsoft.Extensions.Configuration;

namespace MachineBoxingManagement.Tests.BoxInService
{
    [TestClass()]
    public class BoxInServiceTests
    {
        private Mock<CAEDB01Context> _mockContext = new Mock<CAEDB01Context>();
        private Mock<IBoxOutService> _mockBoxout = new Mock<IBoxOutService>();
        private IConfiguration _configuration;
        private MachineBoxingManagement.Web.Services.Implements.BoxInService _boxinService;

        public BoxInServiceTests()
        {
            /*Mocking .Net Core IConfiguration： 
            https://carlpaton.github.io/2021/01/mocking-net-core-iconfiguration/
             */
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();

            var dbContextOptions = new DbContextOptionsBuilder<CAEDB01Context>()
                       .UseNpgsql(_configuration.GetValue<string>("ConnectionStrings:CAEDB01Connection"))
                       .Options;
            var dbContext = new CAEDB01Context(dbContextOptions);

            _mockContext.Setup(m => m.BoxingLocation).Returns(dbContext.BoxingLocation);
            _mockContext.Setup(m => m.BoxingOption).Returns(dbContext.BoxingOption);
            _mockContext.Setup(m => m.BoxingStyle).Returns(dbContext.BoxingStyle);


            ///*Mock IBoxOutService*/
            //_mockBoxout = new Mock<IBoxOutService>();
            //var instockInfo = new Web.Models.Dto.InstockInfo()
            //{
            //    PN = "90NB0S51-T00070",
            //    CDT = "2021-06-21 13:59:53",
            //    SPEC_NOTE = "AU UX482E 1165G7/00T1DA/32G/US//UX482EG-1AKA/WOC/V2G/V/WAX/A00#2"
            //};
            //var errMsg = "";
            //_mockBoxout.Setup(o => o.GetInstockTime("testPN", out errMsg)).Returns(instockInfo);//Mock IBoxOutService.GetInstockTime()


            _boxinService = new MachineBoxingManagement.Web.Services.Implements.BoxInService(_mockContext.Object, _configuration, _mockBoxout.Object);
        }

        /// <summary>
        /// PartNumber檢核 & 取得PN相關資訊
        /// </summary>
        [TestMethod()]        
        public void ProcessingPN_Ok()
        {
            var UserName = "Homer_Chen";
            var PartNumber = "90NB0S51-T00070";//料號
            var Ssn = "";
            var LocationId = 1;
            var MachineOptionId = 1;
            var MachineStyleId = 1;
            var InstockInfo = new Web.Models.Dto.InstockInfo()
            {
                PN = "90NB0S51-T00070",
                CDT = "2021-06-21 13:59:53",
                SPEC_NOTE = "AU UX482E 1165G7/00T1DA/32G/US//UX482EG-1AKA/WOC/V2G/V/WAX/A00#2"
            };
            var Res = new Tuple<PartNumber_Model_Desc, string>(null, "");

            var errMsg = "";
            Mock<IBoxOutService> mockBoxout = new Mock<IBoxOutService>();
            mockBoxout.Setup(o => o.GetInstockTime(PartNumber, out errMsg)).Returns(InstockInfo);
            _boxinService = new MachineBoxingManagement.Web.Services.Implements.BoxInService(_mockContext.Object, _configuration, mockBoxout.Object);

            try
            {
                Res = _boxinService.ProcessingPN(UserName, PartNumber, Ssn, LocationId, MachineOptionId, MachineStyleId).Result;
            }
            catch (Exception Ex)
            {
                Assert.Fail(Ex.Message);
            }

            Assert.AreEqual(Res.Item2, string.Empty);
        }

        /// <summary>
        /// 計算最新裝箱系列相關資訊(最新箱號. 裝箱數量資訊(暫存+DB). 烏龜車層數)
        /// </summary>
        [TestMethod()]
        public void GetMachineInBoxInfo_Get_Info()
        {
            var BoxingSeries = "UX4_PR";//箱名
            var LocationId = 1;
            var Res = new Stocking_Info();
            var ErrMsg = string.Empty;

            try
            {
                Res = _boxinService.GetBoxingSerialInfo(BoxingSeries, LocationId, out ErrMsg);
            }
            catch (Exception Ex)
            {
                Assert.Fail(Ex.Message);                
            }

            Assert.AreEqual(ErrMsg, string.Empty);
            Assert.IsNotNull(Res);
        }

        /// <summary>
        /// 計算最新裝箱系列相關資訊(最新箱號. 裝箱數量資訊(暫存+DB). 烏龜車層數) By 指定箱號
        /// </summary>
        [TestMethod()]
        public void GetMachineInBoxInfoByBoxSerial_Get_Info()
        {
            var BoxingSeries = "UX4_PR";//箱名
            var LocationId = 1;
            var BoxSerial = 1;
            var Res = new Tuple<Stocking_Info, string>(null, "");

            try
            {
                Res = _boxinService.Get_Machine_In_Box_Info_By_BoxSerial(BoxingSeries, LocationId, BoxSerial).Result;
            }
            catch (Exception Ex)
            {
                Assert.Fail(Ex.Message);
            }

            Assert.AreEqual(Res.Item2, string.Empty);
            Assert.IsNotNull(Res.Item1);
        }

        /// <summary>
        /// 取得箱名
        /// </summary>
        [TestMethod()]
        public void GetBoxingName_Get_Info()
        {
            var PartNumber = "90NB0S51-T00070";//料號
            var Model = "test";
            var MachineOptionId = 1;
            var MachineStyleId = 1;
            var InsDate = DateTime.Now;
            var Res = string.Empty;
            var ErrMsg = string.Empty;

            try
            {
                Res = _boxinService.GetBoxingName(PartNumber, Model, MachineOptionId, MachineStyleId, InsDate, out ErrMsg);
            }
            catch (Exception Ex)
            {
                Assert.Fail(Ex.Message);
            }

            Assert.IsNotNull(Res);
            Assert.AreEqual(ErrMsg, string.Empty);
        }

        /// <summary>
        /// 取得(非最新)裝箱系列相關資訊(箱號. 裝箱數量. 烏龜車層數)
        /// </summary>
        [TestMethod()]
        public void GetBoxingSerialInfo_Get_Info()
        {
            var BoxingSeries = "UX4_PR";//箱名
            var LocationId = 1;
            var ErrMsg = string.Empty;
            var Res = new Stocking_Info();

            try
            {
                Res = _boxinService.GetBoxingSerialInfo(BoxingSeries, LocationId, out ErrMsg);
            }
            catch (Exception Ex)
            {
                Assert.Fail(Ex.Message);        
            }

            Assert.IsNotNull(Res);
            Assert.AreEqual(ErrMsg, string.Empty);
        }

        /// <summary>
        /// [裝箱維護]批次儲存機台
        /// </summary>
        [TestMethod()]
        public void SaveBoxingInfos_Ok()
        {
            var Data = new List<PartNumber_Model_Desc> {};

            try
            {
                var Res = _boxinService.SaveBoxingInfos(Data).Result;
            }
            catch (Exception Ex)
            {
                Assert.Fail(Ex.Message);                
            }
        }
    }
}