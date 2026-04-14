using Web.Application.Contracts.DTO.Input.RBAC;
using Web.Application.Contracts.DTO.Output.RBAC;
using Web.Application.Contracts.IServices.RBAC;
using Web.Domain.Entities.RBAC;
using Web.Infrastructure.Repositories;
using Yitter.IdGenerator;

namespace Web.Application.Services.RBAC
{
    public class RoleService : IRoleService
    {
        private readonly IBaseRepository<UserRole> _userRoleRepo;
        private readonly IBaseRepository<Role> _roleRepo;

        public RoleService(
            IBaseRepository<UserRole> userRoleRepo,
            IBaseRepository<Role> roleRepo)
        {
            _userRoleRepo = userRoleRepo;
            _roleRepo = roleRepo;
        }

        /// <summary>
        /// 分配角色
        /// </summary>
        public async Task<bool> AssignRoleAsync(AssignRoleInput input)
        {
            if (input.UserId <= 0)
                throw new Exception("用户ID不合法");
            if (input.RoleId <= 0)
                throw new Exception("角色ID不合法");

            var exists = await _userRoleRepo.GetValue(
                ur => ur.UserId == input.UserId && ur.RoleId == input.RoleId);
            if (exists != null) throw new Exception("该角色已分配");

            var userRole = new UserRole
            {
                Id = YitIdHelper.NextId(),
                UserId = input.UserId,
                RoleId = input.RoleId,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };

            return await _userRoleRepo.AddRange(userRole) > 0;
        }

        /// <summary>
        /// 撤销角色
        /// </summary>
        public async Task<bool> RevokeRoleAsync(AssignRoleInput input)
        {
            if (input.UserId <= 0)
                throw new Exception("用户ID不合法");
            if (input.RoleId <= 0)
                throw new Exception("角色ID不合法");

            var userRole = await _userRoleRepo.GetValue(
                ur => ur.UserId == input.UserId && ur.RoleId == input.RoleId);
            if (userRole == null) throw new Exception("该角色未分配");

            return await _userRoleRepo.Delete(userRole.Id) > 0;
        }

        /// <summary>
        /// 查询用户角色列表
        /// </summary>
        public async Task<List<RoleOutput>> GetUserRolesAsync(long userId)
        {
            if (userId <= 0)
                throw new Exception("用户ID不合法");

            var roleIds = _userRoleRepo.GetValues()
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToList();

            var roles = _roleRepo.GetValues()
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => new RoleOutput
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToList();

            return await Task.FromResult(roles);
        }
    }
}