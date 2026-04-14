using AutoMapper;
using MD5Hash;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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

        public AuthService(
            IBaseRepository<User> userRepo,
            IBaseRepository<UserRole> userRoleRepo,
            IBaseRepository<RolePermission> rolePermissionRepo,
            RedisService redis,
            IConfiguration config)
        {
            _userRepo = userRepo;
            _userRoleRepo = userRoleRepo;
            _rolePermissionRepo = rolePermissionRepo;
            _redis = redis;
            _config = config;
        }

        /// <summary>
        /// 注册
        /// </summary>
        public async Task<bool> RegisterAsync(RegisterInput input)
        {
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
            var user = await _userRepo.GetValue(u => u.Username == input.Username);
            if (user == null) throw new Exception("用户名或密码错误");
            if (user.PasswordHash != input.Password.GetMD5())
                throw new Exception("用户名或密码错误");

            // 查询用户权限
            var roleIds = _userRoleRepo.GetValues()
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToList();

            var permissions = _rolePermissionRepo.GetValues()
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => rp.PermissionId.ToString())
                .Distinct()
                .ToList();

            // 生成双 Token
            var accessToken = GenerateAccessToken(user, permissions);
            var refreshToken = GenerateRefreshToken();

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
            // 从 Redis 里找这个 RefreshToken 属于哪个用户
            // 这里用 scan 前缀匹配，所以 key 规则是 refresh:{userId}
            // 前端需要同时传 userId，或者把 userId 编码进 RefreshToken
            // 这里简化处理：RefreshToken 格式 = userId:randomStr
            var parts = input.RefreshToken.Split(':');
            if (parts.Length != 2) throw new Exception("无效的 RefreshToken");

            var userId = long.Parse(parts[0]);
            var key = $"refresh:{userId}";

            var stored = await _redis.GetAsync(key);
            if (stored == null || stored != input.RefreshToken)
                throw new Exception("RefreshToken 已过期或无效，请重新登录");

            var user = await _userRepo.GetModel(userId);
            if (user == null) throw new Exception("用户不存在");

            var roleIds = _userRoleRepo.GetValues()
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToList();

            var permissions = _rolePermissionRepo.GetValues()
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => rp.PermissionId.ToString())
                .Distinct()
                .ToList();

            // 生成新的双 Token，旧的自动覆盖
            var accessToken = GenerateAccessToken(user, permissions);
            var refreshToken = $"{userId}:{GenerateRefreshToken()}";

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

        private string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes)
                .Replace("+", "").Replace("/", "").Replace("=", "");
        }

        private long GenerateId()
        {
            // 简单用时间戳，生产建议换雪花 ID
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}