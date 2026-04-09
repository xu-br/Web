using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Application.Contracts.DTO.ErrorCode
{
    /// <summary>
    /// 接口返回结果
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class ApiResult<T>
    {
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 状态码
        /// </summary>
        public ApiEnum Code { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public T? Data { get; set; }
    }

    /// <summary>
    /// 接口返回结果（无数据）
    /// </summary>
    public class ApiResult : ApiResult<object>
    {

    }
}
