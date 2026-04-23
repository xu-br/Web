using MD5Hash;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Web.Application.Contracts.DTO.Input.RBAC;
using Web.Application.Contracts.DTO.Output.RBAC;
using Web.Application.Contracts.IServices.RBAC;
using Web.Domain.Entities.RBAC;
using Web.Infrastructure.Repositories;
using Web.Infrastructure.Services;
using Yitter.IdGenerator;

namespace Web.Application.Services.RBAC
{
    public class AuthService : IAuthService
    {
        private readonly IBaseRepository<User> _userRepo;
        private readonly IBaseRepository<UserRole> _userRoleRepo;
        private readonly IBaseRepository<RolePermission> _rolePermissionRepo;
        private readonly RedisService _redis;
        private readonly IConfiguration _config;
        private readonly IBaseRepository<Permission> _permissionRepo;

        public AuthService(
            IBaseRepository<User> userRepo,
            IBaseRepository<UserRole> userRoleRepo,
            IBaseRepository<RolePermission> rolePermissionRepo,
            IBaseRepository<Permission> permissionRepo,  // 新增
            RedisService redis,
            IConfiguration config)
        {
            _userRepo = userRepo;
            _userRoleRepo = userRoleRepo;
            _rolePermissionRepo = rolePermissionRepo;
            _permissionRepo = permissionRepo;  // 新增
            _redis = redis;
            _config = config;
        }

        /// <summary>
        /// 注册
        /// </summary>
        public async Task<bool> RegisterAsync(RegisterInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Username))
                throw new Exception("用户名不能为空");
            if (input.Username.Length < 3 || input.Username.Length > 20)
                throw new Exception("用户名长度为3-20位");
            if (string.IsNullOrWhiteSpace(input.DisplayName))
                throw new Exception("显示名称不能为空");
            if (input.DisplayName.Length < 2 || input.DisplayName.Length > 20)
                throw new Exception("显示名称长度为2-20位");
            if (string.IsNullOrWhiteSpace(input.Email))
                throw new Exception("邮箱不能为空");
            if (!Regex.IsMatch(input.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new Exception("邮箱格式不正确");
            if (string.IsNullOrWhiteSpace(input.Password))
                throw new Exception("密码不能为空");
            if (input.Password.Length < 6 || input.Password.Length > 20)
                throw new Exception("密码长度为6-20位");

            // 检查用户名是否已存在
            var exists = await _userRepo.GetValue(u => u.Username == input.Username);
            if (exists != null) throw new Exception("用户名已存在");

            var user = new User
            {
                Id = YitIdHelper.NextId(),
                Username = input.Username,
                DisplayName = input.DisplayName,
                Email = input.Email,
                PasswordHash = input.Password.GetMD5(),
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };

            return await _userRepo.AddRange(user) > 0;
        }

        /// <summary>
        /// 登录
        /// </summary>
        public async Task<LoginOutput> LoginAsync(LoginInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Username))
                throw new Exception("用户名不能为空");
            if (string.IsNullOrWhiteSpace(input.Password))
                throw new Exception("密码不能为空");
            if (input.Password.Length < 6)
                throw new Exception("密码长度不能少于6位");

            var user = await _userRepo.GetValue(u => u.Username == input.Username);
            if (user == null) throw new Exception("用户名或密码错误");
            if (user.PasswordHash != input.Password.GetMD5())
                throw new Exception("用户名或密码错误");

            var permissions = await GetPermissionNamesAsync(user.Id);

            // 生成双 Token
            var accessToken = GenerateAccessToken(user, permissions);
            var refreshToken = GenerateRefreshToken(user.Id);

            var accessExpires = DateTime.Now.AddHours(2);
            var refreshExpires = DateTime.Now.AddDays(7);

            // RefreshToken 存入 Redis，key = refresh:{userId}
            await _redis.SetAsync(
                $"refresh:{user.Id}",
                refreshToken,
                TimeSpan.FromDays(7)
            );

            return new LoginOutput
            {
                AccessToken = accessToken,
                AccessTokenExpires = accessExpires,
                RefreshToken = refreshToken,
                RefreshTokenExpires = refreshExpires,
                UserId = user.Id,
                Username = user.Username,
                DisplayName = user.DisplayName
            };
        }

        /// <summary>
        /// 刷新 Token
        /// </summary>
        public async Task<LoginOutput> RefreshTokenAsync(RefreshTokenInput input)
        {
            if (string.IsNullOrWhiteSpace(input.RefreshToken))
                throw new Exception("RefreshToken不能为空");

            var parts = input.RefreshToken.Split(':');
            if (parts.Length != 2 || !long.TryParse(parts[0], out var userId))
                throw new Exception("无效的 RefreshToken");
            var key = $"refresh:{userId}";

            var stored = await _redis.GetAsync(key);
            if (stored == null || stored != input.RefreshToken)
                throw new Exception("RefreshToken 已过期或无效，请重新登录");

            var user = await _userRepo.GetModel(userId);
            if (user == null) throw new Exception("用户不存在");

            var permissions = await GetPermissionNamesAsync(user.Id);

            // 生成新的双 Token，旧的自动覆盖
            var accessToken = GenerateAccessToken(user, permissions);
            var refreshToken = GenerateRefreshToken(userId);

            var accessExpires = DateTime.Now.AddHours(2);
            var refreshExpires = DateTime.Now.AddDays(7);

            await _redis.SetAsync(key, refreshToken, TimeSpan.FromDays(7));

            return new LoginOutput
            {
                AccessToken = accessToken,
                AccessTokenExpires = accessExpires,
                RefreshToken = refreshToken,
                RefreshTokenExpires = refreshExpires,
                UserId = user.Id,
                Username = user.Username,
                DisplayName = user.DisplayName
            };
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        public async Task<bool> LogoutAsync(long userId)
        {
            return await _redis.DeleteAsync($"refresh:{userId}");
        }

        // ── 私有方法 ─────────────────────────────────────────

        private string GenerateAccessToken(User user, List<string> permissions)
        {
            var claims = new List<Claim>
            {
                new Claim("userId",   user.Id.ToString()),
                new Claim("username", user.Username),
            };
            // 每条权限写入一个 Claim
            permissions.ForEach(p => claims.Add(new Claim("permission", p)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<List<string>> GetPermissionNamesAsync(long userId)
        {
            var roleIds = _userRoleRepo.GetValues()
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToList();

            var permissionIds = _rolePermissionRepo.GetValues()
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => rp.PermissionId)
                .Distinct()
                .ToList();

            return await Task.FromResult(_permissionRepo.GetValues()
                .Where(p => permissionIds.Contains(p.Id))
                .Select(p => p.Name)
                .ToList());
        }

        private string GenerateRefreshToken(long userId)
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            var randomPart = Convert.ToBase64String(bytes)
                .Replace("+", "").Replace("/", "").Replace("=", "");
            return $"{userId}:{randomPart}";
        }

        private long GenerateId()
        {
            // 简单用时间戳，生产建议换雪花 ID
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
