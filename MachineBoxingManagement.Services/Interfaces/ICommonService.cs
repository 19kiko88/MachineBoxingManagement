using CAEService;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineBoxingManagement.Services.Interfaces
{
    public interface ICommonService
    {
        Employee GetEmployeeInfo(string username);
        List<Employee> GetDeptEmployees(string deptname);
    }
}
