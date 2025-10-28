namespace BasicBackend.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            const string H = "X-Request-ID";
            if (!ctx.Request.Headers.TryGetValue(H, out var id) || string.IsNullOrWhiteSpace(id))
            {
                id = Guid.NewGuid().ToString("n");
                ctx.Response.Headers[H] = id;
            }
            ctx.Items[H] = id.ToString();
            await _next(ctx);
        }
    }
}
