using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MachineBoxingManagement.Web.Services.Interfaces;
using System.Linq;
//using System.Text.Json;
using Newtonsoft.Json;

namespace MachineBoxingManagement.Web.Services.Implements
{
    public class JwtService: IJwtService
    {
        private readonly IConfiguration _config;
        private readonly ICommonService _commonService;


        public JwtService(IConfiguration config, ICommonService commonService)
        {
            _commonService = commonService;
            _config = config;
        }

        /// <summary>
        /// 產生JWT
        /// Ref：
        /// https://iter01.com/678852.html
        /// https://blog.miniasp.com/post/2022/02/13/How-to-use-JWT-token-based-auth-in-aspnet-core-60
        /// https://blog.miniasp.com/post/2019/12/16/How-to-use-JWT-token-based-auth-in-aspnet-core-31
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="expireMinutes"></param>
        /// <returns></returns>
        public string CreateToken(string userName, int expireMinutes = 1440)
        {
            var LimitDept = _config["Authorization:Departments"];//授權部門
            var issuer = _config["JwtSettings:Issuer"];
            var signKey = _config["JwtSettings:SignKey"];

            var operatorInfo = _commonService.GetEmployeeInfo(userName);
            var deptMembers = _commonService.GetDeptEmployees(LimitDept).Select(c => new { item_id = c.ID, item_text = $"{c.NameCT}({c.Name})"});

            // Configuring "Claims" to your JWT Token
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userName)); // User.Identity.Name
            claims.Add(new Claim("Name", operatorInfo.Name));
            claims.Add(new Claim("Detp", operatorInfo.Department));
            claims.Add(new Claim("DeptMembers", JsonConvert.SerializeObject(deptMembers)));//部門成員清單存放於payload，24小時候jwt過期才要重取，避免一直重取資料。

            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // JWT ID
            // TODO: You can define your "roles" to your Claims.
            //claims.Add(new Claim("roles", "Admin"));
            //claims.Add(new Claim("roles", "Users"));

            var userClaimsIdentity = new ClaimsIdentity(claims);

            // Create a SymmetricSecurityKey for JWT Token signatures
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

            // HmacSha256 MUST be larger than 128 bits, so the key can't be too short. At least 16 and more characters.
            // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // Create SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                //Audience = issuer, // 由於你的 API 受眾通常沒有區分特別對象，因此通常不太需要設定，也不太需要驗證
                //NotBefore = DateTime.Now, // Default is DateTime.Now
                //IssuedAt = DateTime.Now, // Default is DateTime.Now
                Subject = userClaimsIdentity,
                Expires = DateTime.Now.AddMinutes(expireMinutes),
                SigningCredentials = signingCredentials
            };

            // Generate a JWT securityToken, than get the serialized Token result (string)
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }
    }
}
