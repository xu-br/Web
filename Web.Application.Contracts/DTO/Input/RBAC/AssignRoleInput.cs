using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Application.Contracts.DTO.Input.RBAC
{
    /// <summary>
    /// 分配角色
    /// </summary>
    public class AssignRoleInput
    {
        /// <summary>
        /// 目标用户 ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 要分配的角色 ID
        /// </summary>
        public long RoleId { get; set; }
    }
}
