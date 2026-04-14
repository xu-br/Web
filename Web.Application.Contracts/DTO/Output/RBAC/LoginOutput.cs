using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Application.Contracts.DTO.Output.RBAC
{
    /// <summary>
    /// 登录返回结果
    /// </summary>
    public class LoginOutput
    {
        /// <summary>
        /// 短期访问 Token，有效期 2 小时
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// AccessToken 过期时间
        /// </summary>
        public DateTime AccessTokenExpires { get; set; }

        /// <summary>
        /// 长期刷新 Token，有效期 7 天
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// RefreshToken 过期时间
        /// </summary>
        public DateTime RefreshTokenExpires { get; set; }

        /// <summary>
        /// 用户 ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 登录用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
    }
}
