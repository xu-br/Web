using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Contracts.DTO.Input.RBAC;
using Web.Application.Contracts.DTO.Output.RBAC;

namespace Web.Application.Contracts.IServices.RBAC
{
    /// <summary>
    /// 权限服务接口
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// 给角色绑定权限
        /// </summary>
        Task<bool> AssignPermissionAsync(AssignPermissionInput input);

        /// <summary>
        /// 撤销角色权限
        /// </summary>
        Task<bool> RevokePermissionAsync(AssignPermissionInput input);

        /// <summary>
        /// 查询用户的所有权限
        /// </summary>
        Task<List<PermissionOutput>> GetUserPermissionsAsync(long userId);

    }
}
