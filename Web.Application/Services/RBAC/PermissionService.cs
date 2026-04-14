using Web.Application.Contracts.DTO.Input.RBAC;
using Web.Application.Contracts.DTO.Output.RBAC;
using Web.Application.Contracts.IServices.RBAC;
using Web.Domain.Entities.RBAC;
using Web.Infrastructure.Repositories;
using Yitter.IdGenerator;

namespace Web.Application.Services.RBAC
{
    public class PermissionService : IPermissionService
    {
        private readonly IBaseRepository<RolePermission> _rolePermissionRepo;
        private readonly IBaseRepository<UserRole> _userRoleRepo;
        private readonly IBaseRepository<Permission> _permissionRepo;

        public PermissionService(
            IBaseRepository<RolePermission> rolePermissionRepo,
            IBaseRepository<UserRole> userRoleRepo,
            IBaseRepository<Permission> permissionRepo)
        {
            _rolePermissionRepo = rolePermissionRepo;
            _userRoleRepo = userRoleRepo;
            _permissionRepo = permissionRepo;
        }

        /// <summary>
        /// 给角色绑定权限
        /// </summary>
        public async Task<bool> AssignPermissionAsync(AssignPermissionInput input)
        {
            if (input.RoleId <= 0)
                throw new Exception("角色ID不合法");
            if (input.PermissionId <= 0)
                throw new Exception("权限ID不合法");

            var exists = await _rolePermissionRepo.GetValue(
                rp => rp.RoleId == input.RoleId && rp.PermissionId == input.PermissionId);
            if (exists != null) throw new Exception("该权限已绑定");

            var rolePermission = new RolePermission
            {
                Id = YitIdHelper.NextId(),
                RoleId = input.RoleId,
                PermissionId = input.PermissionId,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };

            return await _rolePermissionRepo.AddRange(rolePermission) > 0;
        }

        /// <summary>
        /// 撤销角色权限
        /// </summary>
        public async Task<bool> RevokePermissionAsync(AssignPermissionInput input)
        {
            if (input.RoleId <= 0)
                throw new Exception("角色ID不合法");
            if (input.PermissionId <= 0)
                throw new Exception("权限ID不合法");

            var rolePermission = await _rolePermissionRepo.GetValue(
                rp => rp.RoleId == input.RoleId && rp.PermissionId == input.PermissionId);
            if (rolePermission == null) throw new Exception("该权限未绑定");

            return await _rolePermissionRepo.Delete(rolePermission.Id) > 0;
        }

        /// <summary>
        /// 查询用户所有权限
        /// </summary>
        public async Task<List<PermissionOutput>> GetUserPermissionsAsync(long userId)
        {
            if (userId <= 0)
                throw new Exception("用户ID不合法");

            var roleIds = _userRoleRepo.GetValues()
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToList();

            var permissionIds = _rolePermissionRepo.GetValues()
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => rp.PermissionId)
                .Distinct()
                .ToList();

            var permissions = _permissionRepo.GetValues()
                .Where(p => permissionIds.Contains(p.Id))
                .Select(p => new PermissionOutput
                {
                    Id = p.Id,
                    Name = p.Name,
                    Resource = p.Resource,
                    Action = p.Action
                })
                .ToList();

            return await Task.FromResult(permissions);
        }
    }
}