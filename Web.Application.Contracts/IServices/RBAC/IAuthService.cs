using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Contracts.DTO.Input.RBAC;
using Web.Application.Contracts.DTO.Output.RBAC;

namespace Web.Application.Contracts.IServices.RBAC
{
    /// <summary>
    /// 认证服务接口
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 用户注册
        /// </summary>
        Task<bool> RegisterAsync(RegisterInput input);

        /// <summary>
        /// 用户登录，返回双 Token
        /// </summary>
        Task<LoginOutput> LoginAsync(LoginInput input);

        /// <summary>
        /// 刷新 Token
        /// </summary>
        Task<LoginOutput> RefreshTokenAsync(RefreshTokenInput input);

        /// <summary>
        /// 退出登录，销毁 Redis 中的 RefreshToken
        /// </summary>
        Task<bool> LogoutAsync(long userId);

    }
}
