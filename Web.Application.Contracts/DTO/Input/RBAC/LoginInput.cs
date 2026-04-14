using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Application.Contracts.DTO.Input.RBAC
{
    /// <summary>
    /// 用户登录
    /// </summary>
    public class LoginInput
    {
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
