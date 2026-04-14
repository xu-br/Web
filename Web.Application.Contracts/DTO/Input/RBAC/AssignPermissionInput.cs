using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Application.Contracts.DTO.Input.RBAC
{
    /// <summary>
    /// 给角色绑定权限
    /// </summary>
    public class AssignPermissionInput
    {
        /// <summary>
        /// 角色 ID
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 权限 ID
        /// </summary>
        public long PermissionId { get; set; }
    }
}
