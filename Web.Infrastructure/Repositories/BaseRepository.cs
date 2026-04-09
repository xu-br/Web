using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Web.Domain.Entities;
using Web.Infrastructure.Data;

namespace Web.Infrastructure.Repositories
{
    /// <summary>
    /// 基础仓储实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseRepository<T> : IBaseRepository<T> where T : Entity
    {
        private readonly MyDbContext _db;

        public BaseRepository(MyDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<int> AddRange(params T[] entities)
        {
            await _db.Set<T>().AddRangeAsync(entities);
            return await _db.SaveChangesAsync();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> Delete(params long[] id)
        {
            var entities = _db.Set<T>().Where(p => id.Contains(p.Id));
            _db.Set<T>().RemoveRange(entities);
            return await _db.SaveChangesAsync();
        }

        /// <summary>
        /// 根据id查询数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T?> GetModel(long id)
        {
            return await _db.Set<T>().FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// 表达式单条件查询
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<T?> GetValue(Expression<Func<T, bool>> exp)
        {
            return await _db.Set<T>().FirstOrDefaultAsync(exp);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetValues()
        {
            return _db.Set<T>().AsQueryable();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<int> UpdateRange(params T[] entities)
        {
            _db.Set<T>().UpdateRange(entities);
            return await _db.SaveChangesAsync();
        }
    }
}
