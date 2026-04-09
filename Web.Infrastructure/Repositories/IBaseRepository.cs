using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Web.Domain.Entities;

namespace Web.Infrastructure.Repositories
{
    /// <summary>
    /// 基础仓储接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseRepository<T> where T : Entity
    {
        /// <summary>
        /// 表达式单条件查询
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<T?> GetValue(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetValues();

        /// <summary>
        /// 根据id查询数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T?> GetModel(long id);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<int> AddRange(params T[] entities);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<int> UpdateRange(params T[] entities);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> Delete(params long[] id);
    }
}
