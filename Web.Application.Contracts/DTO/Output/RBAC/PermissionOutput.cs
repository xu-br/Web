using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Application.Contracts.DTO.Output.RBAC
{
    /// <summary>
    /// 权限信息
    /// </summary>
    public class PermissionOutput
    {
        /// <summary>
        /// 权限 ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 权限名称，格式 resource:action
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 资源模块
        /// </summary>
        public string Resource { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string Action { get; set; }
    }
}
