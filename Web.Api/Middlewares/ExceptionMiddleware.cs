using System.Text.Json;
using Web.Application.Contracts.DTO.ErrorCode;

namespace Web.Api.Middlewares
{
    /// <summary>
    /// 全局异常处理中间件
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // 拦截 403
                if (context.Response.StatusCode == 403 && !context.Response.HasStarted)
                {
                    context.Response.ContentType = "application/json";
                    var result = ApiResultHelper.Fail("无权限访问");
                    var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    await context.Response.WriteAsync(json);
                }

                // 拦截 401
                if (context.Response.StatusCode == 401 && !context.Response.HasStarted)
                {
                    context.Response.ContentType = "application/json";
                    var result = ApiResultHelper.Fail("请先登录");
                    var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    await context.Response.WriteAsync(json);
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200; // 统一返回 200，业务状态码在 body 里

            var result = ex switch
            {
                // 业务异常（主动抛出的 Exception）返回 Fail
                Exception e when e.GetType() == typeof(Exception)
                    => ApiResultHelper.Fail(ex.Message),
                // 其他系统异常返回 Error
                _ => ApiResultHelper.Error(ex.Message)
            };

            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}