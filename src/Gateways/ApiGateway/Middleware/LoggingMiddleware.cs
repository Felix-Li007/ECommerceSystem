namespace ApiGateway.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId = Guid.NewGuid().ToString();
            context.Items["TraceId"] = traceId;

            var requestTime = DateTime.UtcNow;

            _logger.LogInformation(
                "Gateway Request [{TraceId}]: {Method} {Path}{Query} at {Time}",
                traceId,
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString.Value,
                requestTime);

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Gateway Error [{TraceId}] processing {Method} {Path}",
                    traceId,
                    context.Request.Method,
                    context.Request.Path);

                throw;
            }

            var responseTime = DateTime.UtcNow;
            var elapsed = (responseTime - requestTime).TotalMilliseconds;

            _logger.LogInformation(
                "Gateway Response [{TraceId}]: {Method} {Path} - {StatusCode} - {Elapsed}ms",
                traceId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                elapsed);
        }

    }
}