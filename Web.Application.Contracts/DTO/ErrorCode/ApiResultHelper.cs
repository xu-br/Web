using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Application.Contracts.DTO.ErrorCode
{
    public static class ApiResultHelper
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static ApiResult<T> Success<T>(T data, string msg = "操作成功")
        {
            return new ApiResult<T>
            {
                Code = ApiEnum.Success,
                Msg = msg,
                Data = data
            };
        }

        /// <summary>
        /// 操作成功
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static ApiResult Success(string msg = "操作成功")
        {
            return new ApiResult
            {
                Code = ApiEnum.Success,
                Msg = msg,
                Data = null
            };
        }

        /// <summary>
        /// 操作失败
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ApiResult<T> Fail<T>(string msg = "操作失败", T data = default)
        {
            return new ApiResult<T>
            {
                Code = ApiEnum.Fail,
                Msg = msg,
                Data = data
            };
        }

        /// <summary>
        /// 操作失败
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static ApiResult Fail(string msg = "操作失败")
        {
            return new ApiResult
            {
                Code = ApiEnum.Fail,
                Msg = msg,
                Data = null
            };
        }

        /// <summary>
        /// 系统异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ApiResult<T> Error<T>(string msg = "系统异常", T data = default)
        {
            return new ApiResult<T>
            {
                Code = ApiEnum.Error,
                Msg = msg,
                Data = data
            };
        }

        /// <summary>
        /// 系统异常
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static ApiResult Error(string msg = "系统异常")
        {
            return new ApiResult
            {
                Code = ApiEnum.Error,
                Msg = msg,
                Data = null
            };
        }

        /// <summary>
        /// 分页查询成功
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="totalCount"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static ApiPaging<T> PageSuccess<T>(IEnumerable<T> datas, int totalCount, string msg = "查询成功")
        {
            return new ApiPaging<T>
            {
                Code = ApiEnum.Success,
                Msg = msg,
                Datas = datas as List<T> ?? datas.ToList(),
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 分页查询失败
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static ApiPaging<T> PageFail<T>(string msg = "查询失败")
        {
            return new ApiPaging<T>
            {
                Code = ApiEnum.Fail,
                Msg = msg,
                Datas = new List<T>(),
                TotalCount = 0
            };
        }
    }
}
