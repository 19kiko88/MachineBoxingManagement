using Microsoft.VisualStudio.TestTools.UnitTesting;
using MachineBoxingManagement.Web.Services.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using Microsoft.EntityFrameworkCore;
using MachineBoxingManagement.Services.Models;
using MachineBoxingManagement.Web.Services.Interfaces;
using MachineBoxingManagement.Web.Services.Implements;
using MachineBoxingManagement.Repositories.Data;
using MachineBoxingManagement.Web.Models.Dto;
using Microsoft.Extensions.Configuration;

namespace MachineBoxingManagement.Tests.BoxOutService
{
    [TestClass()]
    public class BoxOutServiceTests
    {
        private Mock<CAEDB01Context> _mockContext = new Mock<CAEDB01Context>();
        private IConfiguration _configuration;
        private MachineBoxingManagement.Web.Services.Implements.BoxOutService _boxoutService;
        private CAEDB01Context dbContext;

        public BoxOutServiceTests()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();

            var dbContextOptions = new DbContextOptionsBuilder<CAEDB01Context>()
                       .UseNpgsql(_configuration.GetValue<string>("ConnectionStrings:CAEDB01Connection"))
                       .Options;
            dbContext = new CAEDB01Context(dbContextOptions);

            _boxoutService = new MachineBoxingManagement.Web.Services.Implements.BoxOutService(_mockContext.Object, _configuration);
        }

        /// <summary>
        /// 取得入庫相關資訊(入庫日期. DESC)
        /// </summary>
        [TestMethod()]
        public void GetInstockTime_Get_Info()
        {
            var PartNumber = "90NB0S51-T00070";
            var ErrMsg = string.Empty;

            try
            {
                var Res = _boxoutService.GetInstockTime(PartNumber, out ErrMsg);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.AreEqual(ErrMsg, string.Empty);
        }

        /// <summary>
        /// 機台查詢
        /// </summary>
        [TestMethod()]
        public void QueryMachines_Get_ListData()
        {
            var DataCondition = new QueryRule()
            {
                PartNumber = "90NB0S51-T00070",
                LocationIds = new List<int> { },
                OptionIds = new List<int> { },
                StyleIds = new List<int> { },
                Statuses = new List<int> { },
                Sd_BoxIn = "1911/01/01",
                Ed_BoxIn = "2999/12/31"
            };
            var Favorites = new int[] { };
            var Res = new Tuple<List<PartNumber_Model_Desc>, string>(null, "");
            _mockContext.Setup(m => m.BoxingStatus).Returns(dbContext.BoxingStatus);

            try
            {
                Res = _boxoutService.QueryMachines(DataCondition, Favorites).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.AreEqual(Res.Item2, string.Empty);
        }

        /// <summary>
        /// 取出機台
        /// </summary>
        [TestMethod()]
        public void TakeOutMachines_Get_SuccessCount()
        {
            var UserName = "Homer_Chen";
            var Ids = new int[] { 804, 806 };
            var Res = new Tuple<List<int>, string>(null, "");

            try
            {
                Res = _boxoutService.TakeOutMachines(UserName, Ids).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.AreEqual(Res.Item2, string.Empty);

            if (Res.Item1.Count > 0)
            {
                Assert.AreEqual(Res.Item1.Count, Ids.Length, "取出數量與輸入id數量不符.");
            }
        }

        /// <summary>
        /// 找出同地點 & 箱名 & 箱號的機台，不管是否為查詢條件的料號，變更暫存區狀態
        /// </summary>
        [TestMethod()]
        public void SaveMachineBufferArea_Get_SuccessCount()
        {
            var UserName = "Homer_Chen";
            var Ids = new Dictionary<int, bool>();
            Ids.Add(804, true);
            Ids.Add(806, false);
            var Res = new Tuple<List<int>, string>(null, "");

            try
            {
                Res = _boxoutService.SaveMachineBufferArea(UserName, Ids).Result;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.AreEqual(Res.Item2, string.Empty);
        }
    }
}
