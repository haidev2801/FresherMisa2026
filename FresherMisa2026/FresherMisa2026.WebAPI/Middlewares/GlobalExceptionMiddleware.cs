using FresherMisa2026.Entities;
using MySqlConnector;
using System.Net;
using System.Text.RegularExpressions;
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

        /// <summary>
        /// Xử lý ngoại lệ và trả về phản hồi lỗi chuẩn hóa
        /// </summary>
        /// <param name="context">HttpContext của yêu cầu</param>
        /// <param name="exception">Ngoại lệ đã bắt được</param>
        /// <returns>Task đại diện cho hoạt động xử lý ngoại lệ</returns>
        /// Updated by: Anhs (20/04/2026)
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var isDuplicateEmployeeCode = IsDuplicateEmployeeCodeException(exception);

            // Set status code and content type
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest,
                FormatException => (int)HttpStatusCode.BadRequest,
                _ when isDuplicateEmployeeCode => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            // Create response payload
            var response = new ServiceResponse
            {
                IsSuccess = false,
                Code = context.Response.StatusCode,
                UserMessage = GetUserMessage(exception),
                DevMessage = exception.Message
            };

            // Serialize the response to JSON
            var jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }

        private static string GetUserMessage(Exception exception)
        {
            if (IsDuplicateEmployeeCodeException(exception))
            {
                return "Mã nhân viên đã tồn tại";
            }

            return GetUserMessageByText(exception.Message);
        }

        private static bool IsDuplicateEmployeeCodeException(Exception exception)
        {
            if (exception is MySqlException mySqlException)
            {
                if (mySqlException.Number == 1062)
                {
                    return mySqlException.Message.Contains("UQ_EmployeeCode", StringComparison.OrdinalIgnoreCase)
                        || mySqlException.Message.Contains("EmployeeCode", StringComparison.OrdinalIgnoreCase);
                }

                if (mySqlException.Number == 1644)
                {
                    return mySqlException.Message.Contains("EmployeeCode đã tồn tại", StringComparison.OrdinalIgnoreCase)
                        || mySqlException.Message.Contains("Mã nhân viên đã tồn tại", StringComparison.OrdinalIgnoreCase);
                }
            }

            return exception.Message.Contains("EmployeeCode đã tồn tại", StringComparison.OrdinalIgnoreCase)
                || exception.Message.Contains("Mã nhân viên đã tồn tại", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetUserMessageByText(string devMessage)
        {
            if (string.IsNullOrWhiteSpace(devMessage))
            {
                return "Có lỗi xảy ra vui lòng liên hệ Misa!";
            }

            // Pattern 0: "<entity> not found" => "<Entity> không tồn tại"
            var notFoundMatch = Regex.Match(devMessage, @"^(?<entity>[A-Za-z_]+)\s+not\s+found$", RegexOptions.IgnoreCase);
            if (notFoundMatch.Success)
            {
                var entity = LocalizeEntityName(notFoundMatch.Groups["entity"].Value);
                return $"{entity} không tồn tại";
            }

            // Pattern 1: "<entity> is null" => "<Entity> không tồn tại"
            var isNullMatch = Regex.Match(devMessage, @"^(?<entity>[A-Za-z_]+)\s+is\s+null$", RegexOptions.IgnoreCase);
            if (isNullMatch.Success)
            {
                var entity = LocalizeEntityName(isNullMatch.Groups["entity"].Value);
                return $"{entity} không tồn tại";
            }

            // Pattern 2: "<entity> code is required" => "Mã <entity> không được để trống"
            var codeRequiredMatch = Regex.Match(devMessage, @"^(?<entity>[A-Za-z_]+)\s+code\s+is\s+required$", RegexOptions.IgnoreCase);
            if (codeRequiredMatch.Success)
            {
                var entity = LocalizeEntityName(codeRequiredMatch.Groups["entity"].Value).ToLower();
                return $"Mã {entity} không được để trống";
            }

            return "Có lỗi xảy ra vui lòng liên hệ Misa!";
        }

        private static string LocalizeEntityName(string rawEntity)
        {
            if (string.IsNullOrWhiteSpace(rawEntity))
            {
                return "Bản ghi";
            }

            return rawEntity.Trim().ToLower() switch
            {
                "department" => "Phòng ban",
                "employee" => "Nhân viên",
                "position" => "Vị trí",
                _ => char.ToUpper(rawEntity.Trim()[0]) + rawEntity.Trim()[1..].ToLower()
            };
        }
    }
}

