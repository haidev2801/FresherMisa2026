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

            var statusCode = (int)HttpStatusCode.InternalServerError;
            var userMessage = "Có lỗi xảy ra vui lòng liên hệ Misa!";

            if (exception is MySqlException mysqlException && mysqlException.Number == 1062)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                userMessage = "Mã nhân viên đã tồn tại";
            }

            context.Response.StatusCode = statusCode;

            // Create response payload
            var response = new ServiceResponse
            {
                IsSuccess = false,
                Code = context.Response.StatusCode,
                UserMessage = userMessage,
                DevMessage = exception.Message // Optional: include for dev
            };

            // Serialize the response to JSON
            var jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}

