using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Domain.Entities.RBAC
{
    /// <summary>
    /// 权限实体，表示系统中一个细粒度的操作许可。
    /// 命名约定为 "resource:action"，例如 "users:write"。
    /// </summary>
    public class Permission : Entity
    {
        /// <summary>
        /// 权限的唯一标识名称，格式为 "resource:action"。
        /// 例如："users:write"、"orders:read"。
        /// 数据库层有唯一索引约束。
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 权限所归属的资源模块，例如 "users"、"orders"、"reports"。
        /// 用于按模块聚合展示权限列表。
        /// </summary>
        public string Resource { get; set; } = string.Empty;

        /// <summary>
        /// 对资源执行的操作类型，例如 "read"、"write"、"delete"、"manage"。
        /// 与 Resource 组合唯一标识一条权限。
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// 权限的可读描述，供管理界面展示，例如 "创建或更新用户信息"。
        /// 可为 null，不影响鉴权逻辑。
        /// </summary>
        public string? Description { get; set; }
    }
}
