using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Domain.Entities.RBAC
{
    /// <summary>
    /// 用户-角色关联实体，表示多对多关系的中间表。
    /// 记录"哪个用户在何时由谁分配了哪个角色"的完整审计信息。
    /// </summary>
    public class UserRole : Entity
    {
        /// <summary>
        /// 外键：关联的用户 ID，与 RoleId 共同构成复合主键。
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 外键：关联的角色 ID，与 UserId 共同构成复合主键。
        /// </summary>
        public long RoleId { get; set; }
    }
}
