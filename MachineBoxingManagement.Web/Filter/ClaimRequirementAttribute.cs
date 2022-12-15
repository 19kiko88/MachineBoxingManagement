using MachineBoxingManagement.Repositories.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using MachineBoxingManagement.Web.Services.Interfaces;

namespace MachineBoxingManagement.Web.Filter
{
    public class ClaimRequirementAttribute: TypeFilterAttribute
    {
        public ClaimRequirementAttribute(string claimType = "", string claimValue = "") : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }

    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        readonly Claim _claim;
        private readonly ICommonService _commonService;


        public ClaimRequirementFilter(Claim claim, ICommonService commonService)
        {
            _claim = claim;
            _commonService = commonService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var Operator = context.HttpContext.User.Identity?.Name.Split('\\')[1];
            var Admins = new string[] { "Homer_Chen" };
            var DeptCheck = _commonService.GetDeptEmployees("系統-PC-產品管理中心-專案管理一處-管理一部")?.Where(c => c.Name == Operator) != null || Admins.Contains(Operator);
            if (!DeptCheck)
            {
                context.Result =
                    new BadRequestObjectResult(new { message = "此登入帳號非隸屬於 [系統-PC-產品管理中心-專案管理一處-管理一部] !!!" });
                    //new BadRequestResult();
                    //new JsonResult(new { success = false, message = "此登入帳號非隸屬於 [系統-PC-產品管理中心-專案管理一處-管理一部] !!!" });
            }
        }
    }
}
