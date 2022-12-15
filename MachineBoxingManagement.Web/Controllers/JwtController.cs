//using MachineBoxingManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalLibrary.Models;
using MachineBoxingManagement.Web.Services.Interfaces;

namespace MachineBoxingManagement.Web.Controllers
{

    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class JwtController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly ICommonService _commonService;


        public JwtController(IJwtService jwtService, ICommonService commonService, IConfiguration configuration) 
        {
            _jwtService = jwtService;
            _configuration = configuration;
            _commonService = commonService;
        }

        /// <summary>
        /// 取得JWT
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<Result<string>> GetJWT()
        {
            var Operator = HttpContext.User.Identity.Name.Split('\\')[1];
            var result = new Result<string>() { Success = false };
            var Jwt = "";

            try
            {
                Jwt = _jwtService.CreateToken(Operator);
                result.Content = Jwt;
                result.Success = true;
            }
            catch (System.Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// (目前先保留暫不使用改由前端直接判斷JWT是否過期)
        /// 
        /// 驗證JWT(是否過期)
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        [HttpGet]
        /*
         *Ref：https://jasonwatmore.com/post/2022/01/19/net-6-create-and-validate-jwt-tokens-use-custom-jwt-middleware
         */
        public async Task<Result<bool>> JwtValidate(string jwt)
        {
            var result = new Result<bool>() { Success = false };
            var Res = false;

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SignKey"]);
            try
            {
                tokenHandler.ValidateToken(jwt, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero////過期時間容錯值，解決伺服器端時間不同步問題（秒）
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                Res = true;

                result.Content = Res;
                result.Success = true;

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
