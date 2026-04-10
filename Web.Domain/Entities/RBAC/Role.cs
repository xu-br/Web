using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Domain.Entities.RBAC
{
    /// <summary>
    /// 角色实体，代表一组权限的集合。
    /// 用户通过被分配角色来间接获得权限，而非直接绑定权限。
    /// </summary>
    public class Role : Entity
    {
        /// <summary>
        /// 角色的唯一名称，例如 "Admin"、"Manager"、"Viewer"。
        /// 数据库层有唯一索引约束。
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 角色的可读描述，说明该角色的职责范围。
        /// 例如："系统管理员，拥有全部操作权限"。
        /// 可为 null。
        /// </summary>
        public string? Description { get; set; }
    }
}
