using FresherMisa2026.Entities;
using MySqlConnector;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FresherMisa2026.WebAPI.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly Dictionary<string, string> _uniqueKeyFriendlyNames = new(StringComparer.OrdinalIgnoreCase)
        {
            { "UQ_EmployeeCode",   "Mã nhân viên" },
            { "UQ_DepartmentCode", "Mã phòng ban" },
            { "UQ_PositionCode",   "Mã chức vụ" },
        };

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
                case MySqlException mySqlException when mySqlException.Number == 1062:
                    statusCode = (int)HttpStatusCode.Conflict;
                    userMessage = BuildDuplicateKeyMessage(mySqlException.Message);
                    devMessage = mySqlException.Message;
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

        /// <summary>
        /// Parse message MySQL 1062 để build message
        /// MySQL format: "Duplicate entry 'EMP001' for key 'employee.UQ_EmployeeCode'"
        /// </summary>
        private static string BuildDuplicateKeyMessage(string mysqlMessage)
        {
            var match = Regex.Match(
                mysqlMessage,
                @"Duplicate entry '(.+?)' for key '(.+?)'",
                RegexOptions.IgnoreCase);

            if (!match.Success)
                return "Dữ liệu đã tồn tại trong hệ thống";

            var entryValue = match.Groups[1].Value;
            // key có thể có dạng 'table.UQ_IndexName' → chỉ lấy phần sau dấu chấm
            var rawKeyName = match.Groups[2].Value.Split('.').Last();

            var fieldName = _uniqueKeyFriendlyNames.TryGetValue(rawKeyName, out var friendly)
                ? friendly
                : rawKeyName;

            return $"{fieldName} '{entryValue}' đã tồn tại";
        }
    }
}

