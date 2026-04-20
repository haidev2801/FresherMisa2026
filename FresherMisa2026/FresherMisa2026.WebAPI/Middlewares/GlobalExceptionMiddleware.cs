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
            // Set status code and content type
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = IsDuplicateEmployeeCodeException(exception)
                ? (int)HttpStatusCode.BadRequest
                : (int)HttpStatusCode.InternalServerError;

            // Create response payload
            var response = new ServiceResponse
            {
                IsSuccess = false,
                Code = context.Response.StatusCode,
                UserMessage = IsDuplicateEmployeeCodeException(exception)
                    ? "Mã nhân viên đã tồn tại"
                    : "Có lỗi xảy ra vui lòng liên hệ Misa!",
                DevMessage = exception.Message // Optional: include for dev
            };

            // Serialize the response to JSON
            var jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }

        private static bool IsDuplicateEmployeeCodeException(Exception exception)
        {
            if (exception is not MySqlException mySqlException)
            {
                return false;
            }

            var isDuplicateErrorCode = mySqlException.Number == 1062 || mySqlException.Number == 1644;
            if (!isDuplicateErrorCode)
            {
                return false;
            }

            return mySqlException.Message.Contains("EmployeeCode", StringComparison.OrdinalIgnoreCase)
                || mySqlException.Message.Contains("UQ_EmployeeCode", StringComparison.OrdinalIgnoreCase);
        }
    }
}
