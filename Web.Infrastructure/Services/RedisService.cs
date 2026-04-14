using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Infrastructure.Services
{
    /// <summary>
    /// Redis 服务
    /// </summary>
    public class RedisService
    {
        private readonly IDatabase _db;

        public RedisService(string connectionString)
        {
            var redis = ConnectionMultiplexer.Connect(connectionString);
            _db = redis.GetDatabase();
        }

        /// <summary>
        /// 存值，并设置过期时间
        /// </summary>
        public async Task SetAsync(string key, string value, TimeSpan expiry)
        {
            await _db.StringSetAsync(key, value, expiry);
        }

        /// <summary>
        /// 取值
        /// </summary>
        public async Task<string?> GetAsync(string key)
        {
            var value = await _db.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }

        /// <summary>
        /// 删除
        /// </summary>
        public async Task<bool> DeleteAsync(string key)
        {
            return await _db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 判断 key 是否存在
        /// </summary>
        public async Task<bool> ExistsAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
        }
    }
}
