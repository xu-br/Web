using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // 拦截 403
                if (context.Response.StatusCode == 403 && !context.Response.HasStarted)
                {
                    _logger.LogWarning("访问被拒绝: Path={Path}, Method={Method}, RemoteIP={RemoteIP}",
                        context.Request.Path, context.Request.Method, context.Connection.RemoteIpAddress);

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
                    _logger.LogWarning("未授权访问: Path={Path}, Method={Method}, RemoteIP={RemoteIP}",
                        context.Request.Path, context.Request.Method, context.Connection.RemoteIpAddress);

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
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger<ExceptionMiddleware> logger)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200; // 统一返回 200，业务状态码在 body 里

            var result = ex switch
            {
                // 业务异常（主动抛出的 Exception）返回 Fail
                Exception e when e.GetType() == typeof(Exception) => HandleBusinessException(ex, context, logger),
                // 其他系统异常返回 Error
                _ => HandleSystemException(ex, context, logger)
            };

            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        private static object HandleBusinessException(Exception ex, HttpContext context, ILogger<ExceptionMiddleware> logger)
        {
            logger.LogWarning("业务异常: Message={Message}, Path={Path}, Method={Method}",
                ex.Message, context.Request.Path, context.Request.Method);

            return ApiResultHelper.Fail(ex.Message);
        }

        private static object HandleSystemException(Exception ex, HttpContext context, ILogger<ExceptionMiddleware> logger)
        {
            logger.LogError(ex, "系统异常: Message={Message}, Path={Path}, Method={Method}, ExceptionType={ExceptionType}",
                ex.Message, context.Request.Path, context.Request.Method, ex.GetType().Name);

            return ApiResultHelper.Error(ex.Message);
        }
    }
}