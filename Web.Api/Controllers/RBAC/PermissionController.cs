using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Contracts.DTO.Input.RBAC;
using Web.Application.Contracts.DTO.ErrorCode;
using Web.Application.Contracts.IServices.RBAC;

namespace Web.Api.Controllers.RBAC
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// 给角色绑定权限
        /// </summary>
        [HttpPost("assign")]
        public async Task<IActionResult> Assign([FromBody] AssignPermissionInput input)
        {
            var result = await _permissionService.AssignPermissionAsync(input);
            return Ok(ApiResultHelper.Success(result));
        }

        /// <summary>
        /// 撤销角色权限
        /// </summary>
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] AssignPermissionInput input)
        {
            var result = await _permissionService.RevokePermissionAsync(input);
            return Ok(ApiResultHelper.Success(result));
        }

        /// <summary>
        /// 查询用户所有权限
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPermissions(long userId)
        {
            var result = await _permissionService.GetUserPermissionsAsync(userId);
            return Ok(ApiResultHelper.Success(result));
        }
    }
}