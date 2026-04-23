using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Domain.Entities.RBAC
{
    /// <summary>
    /// 用户实体，表示系统的登录主体。
    /// 用户本身不直接持有权限，而是通过角色间接获得。
    /// </summary>
    public class User : Entity
    {
        /// <summary>
        /// 登录用户名，全局唯一，仅用于身份认证，不对外展示。
        /// 数据库层有唯一索引约束。
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 显示名称，用于界面展示，例如评论署名、个人主页标题等。
        /// 允许重复，不参与登录逻辑，注册时必填。
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 用户邮箱地址，全局唯一，可用于找回密码或通知。
        /// 数据库层有唯一索引约束。
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 经过 MD5 处理后的密码摘要。
        /// 原始密码不应以任何形式持久化，鉴权时使用相同的 MD5 规则进行比对。
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;
    }
}
