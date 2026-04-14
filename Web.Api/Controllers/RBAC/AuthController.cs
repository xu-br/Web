using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Contracts.DTO.Input.RBAC;
using Web.Application.Contracts.DTO.ErrorCode;
using Web.Application.Contracts.IServices.RBAC;

namespace Web.Api.Controllers.RBAC
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// 注册
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterInput input)
        {
            var result = await _authService.RegisterAsync(input);
            return Ok(ApiResultHelper.Success(result));
        }

        /// <summary>
        /// 登录
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInput input)
        {
            var result = await _authService.LoginAsync(input);
            return Ok(ApiResultHelper.Success(result));
        }

        /// <summary>
        /// 刷新 Token
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenInput input)
        {
            var result = await _authService.RefreshTokenAsync(input);
            return Ok(ApiResultHelper.Success(result));
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = long.Parse(User.FindFirst("userId")!.Value);
            var result = await _authService.LogoutAsync(userId);
            return Ok(ApiResultHelper.Success(result));
        }
    }
}