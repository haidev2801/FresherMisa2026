using System.Diagnostics;

namespace FresherMisa2026.WebAPI.Middlewares
{

    /// <summary>
    /// Middleware để log thông tin request và response, 
    /// bao gồm method, path, status code và thời gian xử lý.
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path;

            _logger.LogInformation("→ {Method} {Path}", method, path);

            var sw = Stopwatch.StartNew();
            await _next(context);
            sw.Stop();

            _logger.LogInformation("← {StatusCode} {Method} {Path} ({Elapsed}ms)",
                context.Response.StatusCode, method, path, sw.ElapsedMilliseconds);
        }
    }
}
