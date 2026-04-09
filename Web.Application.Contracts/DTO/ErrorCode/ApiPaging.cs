using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Application.Contracts.DTO.ErrorCode
{
    /// <summary>
    /// 接口返回结果（分页）
    /// </summary>
    /// <typeparam name="T"> 返回数据类型 </typeparam>
    public class ApiPaging<T> : ApiResult<T>
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T> Datas { get; set; } = new List<T>();
    }
}
