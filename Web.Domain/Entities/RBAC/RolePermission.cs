using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Domain.Entities.RBAC
{
    /// <summary>
    /// 角色-权限关联实体，表示多对多关系的中间表。
    /// 定义某个角色具备哪些操作权限。
    /// </summary>
    public class RolePermission : Entity
    {
        /// <summary>
        /// 外键：关联的角色 ID，与 PermissionId 共同构成复合主键。
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 外键：关联的权限 ID，与 RoleId 共同构成复合主键。
        /// </summary>
        public long PermissionId { get; set; }
    }
}
