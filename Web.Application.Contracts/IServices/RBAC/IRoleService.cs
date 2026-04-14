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
    /// 角色服务接口
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// 给用户分配角色
        /// </summary>
        Task<bool> AssignRoleAsync(AssignRoleInput input);

        /// <summary>
        /// 撤销用户角色
        /// </summary>
        Task<bool> RevokeRoleAsync(AssignRoleInput input);

        /// <summary>
        /// 查询用户的角色列表
        /// </summary>
        Task<List<RoleOutput>> GetUserRolesAsync(long userId);

    }
}
