using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using MachineBoxingManagement.Repositories.Models;
using MachineBoxingManagement.Web.Services.Interfaces;
using UniversalLibrary.Models;
using System.Linq;
using System;
using Microsoft.Extensions.Configuration;
using CAEService;
using System.Collections.Generic;

namespace MachineBoxingManagement.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class CommonController : ControllerBase
{
    private readonly ICommonService _commonService;
    private readonly IConfiguration _configuration;


    public CommonController(ICommonService commonService, IConfiguration configuration)
    {
        _commonService = commonService;
        _configuration = configuration;
    }

    /// <summary>
    /// 取得UserName
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    public async Task<Result<string>> GetUserName()
    {
        var result = new Result<string>() { Success = false };
        try
        {
            result.Content = User.Identity.Name.Split('\\')[1];
            result.Success = true;
        }
        catch (System.Exception ex)
        {
            result.Message = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 下拉選單[庫房]
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    public async Task<Result<BoxingLocation[]>> GetBoxingLocations()
    {
        var result = new Result<BoxingLocation[]>() { Success = false };
        try
        {
            result.Content = await _commonService.GetBoxingLocations();
            result.Success = true;
        }
        catch (System.Exception ex)
        {
            result.Message = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 下拉選單[機台選項]
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    public async Task<Result<BoxingOption[]>> GetBoxingOptions()
    {
        var result = new Result<BoxingOption[]>() { Success = false };
        try
        {
            result.Content = await _commonService.GetBoxingOptions();
            result.Success = true;
        }
        catch (System.Exception ex)
        {
            result.Message = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 下拉選單[樣式選項]
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    public async Task<Result<BoxingStyle[]>> GetBoxingStyle()
    {
        var result = new Result<BoxingStyle[]>() { Success = false };
        try
        {
            result.Content = await _commonService.GetBoxingStyle();
            result.Success = true;
        }
        catch (System.Exception ex)
        {
            result.Message = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 下拉選單[機台狀態]
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    public async Task<Result<BoxingStatus[]>> GetBoxingStatus()
    {
        var result = new Result<BoxingStatus[]>() { Success = false };
        try
        {
            result.Content = await _commonService.GetBoxingStatus();
            result.Success = true;
        }
        catch (System.Exception ex)
        {
            result.Message = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 取得指定部門(系統-PC-產品管理中心-專案管理一處-管理一部)的人員清單
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<Result<List<Employee>>> GetEmployeeInfo() 
    {
        var result = new Result<List<Employee>>() { Success = false };
        try
        {
            result.Content = _commonService.GetDeptEmployees(_configuration["Authorization:Departments"]);
            result.Success = true;
        }
        catch (System.Exception ex)
        {
            result.Message = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 部門權限驗證
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<Result<bool>> DeptCheck()
    {
        var Operator = HttpContext.User.Identity.Name.Split('\\')[1];
        var result = new Result<bool>() { Success = false };
        var LimitDept = _configuration["Authorization:Departments"];//授權部門
        var Admins = _configuration.GetSection("Authorization:Users").Get<List<string>>().ToArray();//管理員
        var Res = false;

        try
        {
            var OperatorInfo = _commonService.GetEmployeeInfo(Operator);

            if (OperatorInfo != null && OperatorInfo.Department == LimitDept || Admins.Contains(Operator))
            {//通過部門驗證，取得JWT
                Res = true;
            }
            else
            {
                result.Message = $"無使用權限，MBM僅供[{LimitDept}]使用";
            }

            result.Content = Res;
            result.Success = true;
        }
        catch (System.Exception ex)
        {
            result.Message = ex.Message;
        }

        return result;
    }
}

