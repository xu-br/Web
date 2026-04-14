using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Application.Contracts.DTO.Input.RBAC
{
    /// <summary>
    /// 刷新 Token
    /// </summary>
    public class RefreshTokenInput
    {
        /// <summary>
        /// 客户端持有的 RefreshToken
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
