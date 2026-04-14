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
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// 分配角色
        /// </summary>
        [HttpPost("assign")]
        public async Task<IActionResult> Assign([FromBody] AssignRoleInput input)
        {
            var result = await _roleService.AssignRoleAsync(input);
            return Ok(ApiResultHelper.Success(result));
        }

        /// <summary>
        /// 撤销角色
        /// </summary>
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] AssignRoleInput input)
        {
            var result = await _roleService.RevokeRoleAsync(input);
            return Ok(ApiResultHelper.Success(result));
        }

        /// <summary>
        /// 查询用户角色列表
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRoles(long userId)
        {
            var result = await _roleService.GetUserRolesAsync(userId);
            return Ok(ApiResultHelper.Success(result));
        }
    }
}