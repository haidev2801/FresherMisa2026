using FresherMisa2026.Entities;
using System.Net;
using System.Text.Json;

namespace FresherMisa2026.WebAPI.Middlewares
{
    /// <summary>
    /// Middleware xử lý exception toàn cục
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Khởi tạo middleware
        /// </summary>
        /// <param name="next">Middleware kế tiếp</param>
        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Thực thi middleware và bắt lỗi toàn cục
        /// </summary>
        /// <param name="context">HttpContext hiện tại</param>
        /// <returns>Task xử lý request</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                Console.WriteLine("Before run middleware");
                // Pass the request to the next middleware/component
                await _next(context);
                Console.WriteLine("After run middleware");
            }
            catch (Exception ex)
            {
                // Handle the exception globally
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Chuẩn hóa phản hồi khi có lỗi hệ thống
        /// </summary>
        /// <param name="context">HttpContext hiện tại</param>
        /// <param name="exception">Exception phát sinh</param>
        /// <returns>Task ghi phản hồi lỗi</returns>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Set status code and content type
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Create response payload
            var response = new ServiceResponse
            {
                IsSuccess = false,
                Code = context.Response.StatusCode,
                UserMessage = "Có lỗi xảy ra vui lòng liên hệ Misa!",
                DevMessage = exception.Message // Optional: include for dev
            };

            // Serialize the response to JSON
            var jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}

