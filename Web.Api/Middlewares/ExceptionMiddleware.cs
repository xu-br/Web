using System.Text.Json;
using Web.Application.Contracts.DTO.ErrorCode;

namespace Web.Api.Middlewares
{
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