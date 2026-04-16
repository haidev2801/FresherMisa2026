using FresherMisa2026.Entities;
using MySqlConnector;
using System.Net;
using System.Text.Json;

namespace FresherMisa2026.WebAPI.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var userMessage = "Có lỗi xảy ra vui lòng liên hệ Misa!";
            var devMessage = exception.Message;

            switch (exception)
            {
                case KeyNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    userMessage = exception.Message;
                    break;
                case ArgumentException:
                case InvalidOperationException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    userMessage = exception.Message;
                    break;
                case MySqlException mySqlException when mySqlException.SqlState == "45000":
                    statusCode = mySqlException.Message.Contains("không tồn tại", StringComparison.OrdinalIgnoreCase)
                        ? (int)HttpStatusCode.NotFound
                        : (int)HttpStatusCode.BadRequest;
                    userMessage = mySqlException.Message;
                    devMessage = mySqlException.Message;
                    break;
            }

            context.Response.StatusCode = statusCode;

            var response = new ServiceResponse
            {
                IsSuccess = false,
                Code = statusCode,
                UserMessage = userMessage,
                DevMessage = devMessage
            };

            var jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}

