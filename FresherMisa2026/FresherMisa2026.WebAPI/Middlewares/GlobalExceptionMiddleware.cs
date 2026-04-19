using FresherMisa2026.Entities;
using System.Net;
using System.Text.Json;

namespace FresherMisa2026.WebAPI.Middlewares
{
    /// <summary>
    /// Middleware bắt mọi exception không được catch ở tầng Controller/Service.
    /// Trả về ServiceResponse ở dạng JSON với Code = 500 và thông điệp người dùng/Dev.
    /// Gắn middleware này ở Program.cs để áp dụng cho toàn bộ pipeline.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Phương thức chính của middleware. Thực hiện try/catch xung quanh _next(context).
        /// Nếu có exception sẽ xử lý bằng HandleExceptionAsync.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                Console.WriteLine("Before run middleware");
                // Chuyển tiếp request đến middleware/component tiếp theo
                await _next(context);
                Console.WriteLine("After run middleware");
            }
            catch (Exception ex)
            {
                // Xử lý exception toàn cục
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Xử lý exception: thiết lập status code 500 và trả ServiceResponse dạng JSON.
        /// Lưu ý: DevMessage chứa exception.Message; nếu muốn thêm chi tiết có thể thêm stack trace khi đang ở môi trường dev.
        /// </summary>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode = (int)HttpStatusCode.InternalServerError;
            string userMessage = "Có lỗi xảy ra vui lòng liên hệ Misa!";

            // Bắt duplicate
            if (exception is DuplicateException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                userMessage = exception.Message;
            }

            context.Response.StatusCode = statusCode;

            var response = new ServiceResponse
            {
                IsSuccess = false,
                Code = statusCode,
                UserMessage = userMessage,
                DevMessage = exception.Message
            };

            var jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}

