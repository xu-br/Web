using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.Application.Contracts.DTO.ErrorCode;

namespace Web.Api.Filters
{
    /// <summary>
    /// 权限校验特性
    /// </summary>
    public class PermissionAttribute : TypeFilterAttribute
    {
        public PermissionAttribute(string permission) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { permission };
        }
    }

    /// <summary>
    /// 权限校验过滤器
    /// </summary>
    public class PermissionFilter : IAuthorizationFilter
    {
        private readonly string _permission;

        public PermissionFilter(string permission)
        {
            _permission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new OkObjectResult(ApiResultHelper.Fail("请先登录"));
                return;
            }

            var hasPermission = user.Claims
                .Any(c => c.Type == "permission" && c.Value == _permission);

            if (!hasPermission)
            {
                context.Result = new OkObjectResult(ApiResultHelper.Fail("无权限访问"));
            }
        }
    }
}