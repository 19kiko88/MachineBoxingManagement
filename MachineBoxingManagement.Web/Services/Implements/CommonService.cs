using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MachineBoxingManagement.Repositories.Data;
using MachineBoxingManagement.Repositories.Models;
using MachineBoxingManagement.Services;
using MachineBoxingManagement.Web.Services.Interfaces;

namespace MachineBoxingManagement.Web.Services.Implements
{
    public class CommonService : ICommonService
    {
        private readonly CAEDB01Context _context;
        private readonly MachineBoxingManagement.Services.BoxInService _boxinService;
        private readonly IConfiguration _configuration;

        public CommonService(CAEDB01Context context, IConfiguration config)
        {
            _context = context;
            _configuration = config;
            _boxinService = new MachineBoxingManagement.Services.BoxInService(_configuration.GetValue<string>("ConnectionStrings:CAEDB01Connection"), _configuration.GetValue<string>("ConnectionStrings:CAEServiceConnection"));
        }

        public async Task<BoxingLocation[]> GetBoxingLocations() 
        {
            return _context.BoxingLocation.OrderBy(o => o.Id).ToArray();
        }

        public async Task<BoxingOption[]> GetBoxingOptions()
        {
            return _context.BoxingOption.OrderBy(o => o.Id).ToArray();
        }

        public async Task<BoxingStyle[]> GetBoxingStyle()
        {
            return _context.BoxingStyle.OrderBy(o => o.Id).ToArray();
        }

        public async Task<BoxingStatus[]> GetBoxingStatus()
        {
            return _context.BoxingStatus.OrderBy(o => o.Id).ToArray();
        }
    }
}
