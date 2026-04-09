using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Application.Contracts.DTO.ErrorCode
{
    public enum ApiEnum
    {
        /// <summary>
        /// 正常
        /// </summary>
        Success = 200,
        /// <summary>
        /// 错误
        /// </summary>
        Fail = 400,
        /// <summary>
        /// 异常
        /// </summary>
        Error = 500
    }
}
