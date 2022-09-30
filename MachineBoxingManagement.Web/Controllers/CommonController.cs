using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using MachineBoxingManagement.Repositories.Models;
using MachineBoxingManagement.Web.Services.Interfaces;
using UniversalLibrary.Models;

namespace MachineBoxingManagement.Web.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
public class CommonController : ControllerBase
{
    private readonly ICommonService _commonService;

    public CommonController(ICommonService commonService)
    {
        _commonService = commonService;
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
}

