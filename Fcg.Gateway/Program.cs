using System.Net;
using System.Net.Http;
using System.Net.Security;
using Fcg.Gateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Application Insights
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

// Health Checks
builder.Services.AddHealthChecks();

// Reverse Proxy (YARP) + HttpClient tuning para Container Apps
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .ConfigureHttpClient((context, handler) =>
    {
        if (handler is SocketsHttpHandler sockets)
        {
            // Evita problemas de negotiation em alguns ambientes
            sockets.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            // Força HTTP/1.1 (muito útil quando o upstream/ingress não se dá bem com H2)
            sockets.SslOptions.ApplicationProtocols = new List<SslApplicationProtocol>
            {
                SslApplicationProtocol.Http11
            };

            // Se o internal FQDN reclamar de certificado, descomente:
            // sockets.SslOptions.RemoteCertificateValidationCallback =
            //     (_, _, _, _) => true;
        }
    });

var app = builder.Build();

// Middleware de Correlation ID (primeiro)
app.UseMiddleware<CorrelationIdMiddleware>();

// Middleware de Logging estruturado
app.UseMiddleware<RequestLoggingMiddleware>();

// Health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

// Endpoint de informações do gateway
app.MapGet("/", () => Results.Ok(new
{
    service = "FCG Gateway",
    version = "1.0.0",
    status = "running"
}));

// YARP Reverse Proxy
app.MapReverseProxy();

app.Run();
