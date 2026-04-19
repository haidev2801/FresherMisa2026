using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Exceptions;
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            ServiceResponse response;

            // DuplicateKeyException là lỗi nghiệp vụ có thể dự đoán được (race condition):
            // 2 request cùng lúc insert trùng mã → DB ném unique constraint violation.
            // Đây là lỗi của client (dữ liệu trùng) nên trả 400, không phải 500.
            if (exception is DuplicateKeyException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ServiceResponse
                {
                    IsSuccess = false,
                    Code = context.Response.StatusCode,
                    Data = "Dữ liệu đã tồn tại, vui lòng kiểm tra lại",
                    DevMessage = "Duplicate key vi phạm unique constraint"
                };
            }
            else
            {
                // Các lỗi không lường trước → 500, ẩn detail, chỉ log nội bộ
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ServiceResponse
                {
                    IsSuccess = false,
                    Code = context.Response.StatusCode,
                    UserMessage = "Có lỗi xảy ra vui lòng liên hệ Misa!",
                    DevMessage = exception.Message
                };
            }

            // Serialize the response to JSON
            var jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}

