namespace Fcg.Gateway.Middleware;

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
        var startTime = DateTime.UtcNow;
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        try
        {
            _logger.LogInformation(
                "Gateway request started: {Method} {Path}",
                requestMethod,
                requestPath);

            await _next(context);

            var duration = DateTime.UtcNow - startTime;
            var statusCode = context.Response.StatusCode;

            _logger.LogInformation(
                "Gateway request completed: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms",
                requestMethod,
                requestPath,
                statusCode,
                duration.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            var duration = DateTime.UtcNow - startTime;

            _logger.LogError(
                ex,
                "Gateway request failed: {Method} {Path} - Duration: {Duration}ms",
                requestMethod,
                requestPath,
                duration.TotalMilliseconds);

            throw;
        }
    }
}
