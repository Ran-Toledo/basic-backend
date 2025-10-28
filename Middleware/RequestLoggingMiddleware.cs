using BasicBackend.Utilities;

namespace BasicBackend.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger("Requests");
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            var sw = StopwatchSlim.StartNew();
            await _next(ctx);
            _logger.LogInformation("{Method} {Path} => {Status} ({Ms} ms) rid={Rid}",
                ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode,
                sw.ElapsedMilliseconds, ctx.Items["X-Request-ID"]);
        }
    }
}
