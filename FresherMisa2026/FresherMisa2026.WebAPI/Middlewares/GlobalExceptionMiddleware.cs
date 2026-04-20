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
            catch (MySqlException mysqlEx) when (mysqlEx.Number == 1062)
            {
                // Task 3.2: Xử lý race condition - Duplicate entry (MySQL error 1062)
                // Khi unique index bắt trùng ở tầng database
                await HandleDuplicateEntryAsync(context, mysqlEx);
            }
            catch (Exception ex)
            {
                // Handle the exception globally
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Task 3.2: Xử lý duplicate entry từ database-level constraint
        /// MySQL Error 1062 = Duplicate entry for key
        /// Khi 2 request cùng lúc pass được validation ở tầng service,
        /// UNIQUE INDEX sẽ chặn ở tầng database và throw MySqlException.
        /// </summary>
        private static Task HandleDuplicateEntryAsync(HttpContext context, MySqlException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            // Parse tên field bị trùng từ message MySQL
            // Ví dụ: "Duplicate entry 'EMP001' for key 'employee.UQ_EmployeeCode'"
            var userMessage = "Mã đã tồn tại trong hệ thống";
            if (exception.Message.Contains("EmployeeCode", StringComparison.OrdinalIgnoreCase))
            {
                userMessage = "Mã nhân viên đã tồn tại";
            }
            else if (exception.Message.Contains("DepartmentCode", StringComparison.OrdinalIgnoreCase))
            {
                userMessage = "Mã phòng ban đã tồn tại";
            }
            else if (exception.Message.Contains("PositionCode", StringComparison.OrdinalIgnoreCase))
            {
                userMessage = "Mã chức vụ đã tồn tại";
            }

            var response = new ServiceResponse
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                UserMessage = userMessage,
                DevMessage = exception.Message
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }

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

