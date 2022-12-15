using CAEService;
using MachineBoxingManagement.Repositories.Data;
using MachineBoxingManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MachineBoxingManagement.Services
{
    public class CommonService : ICommonService
    {

        private readonly CAEService.OraServiceClient _serviceClient;
        private readonly CAEDB01Context _caEDB01Context;
        private readonly DbContextOptionsBuilder<CAEDB01Context> _dbContextOptionsBuilder;

        public CommonService(string dbconn, string wcfconn)
        {
            _serviceClient = new CAEService.OraServiceClient(CAEService.OraServiceClient.EndpointConfiguration.BasicHttpBinding_IOraService, wcfconn);
            _dbContextOptionsBuilder = new DbContextOptionsBuilder<CAEDB01Context>();
            _dbContextOptionsBuilder.UseNpgsql(dbconn);
            _caEDB01Context = new CAEDB01Context(_dbContextOptionsBuilder.Options);
        }

        public List<Employee> GetDeptEmployees(string deptname)
        {
            var emps = _serviceClient.GetEIPEmployeeDataAsync("Department", deptname, true, true).Result.GetEIPEmployeeDataResult;
            if (emps.IsMatch && emps.Employees.Any(a => a.Quit == "N"))
            {
                return emps.Employees.Where(a => a.Quit == "N").ToList();
            }
            return null;
        }

        public Employee GetEmployeeInfo(string username)
        {
            var emps = _serviceClient.GetEIPEmployeeDataAsync("Name", username, false, true).Result.GetEIPEmployeeDataResult;
            if (emps.IsMatch && emps.Employees.Any(a => a.Quit == "N"))
            {
                return emps.Employees.Where(a => a.Quit == "N").First();
            }
            return null;
        }
    }
}
