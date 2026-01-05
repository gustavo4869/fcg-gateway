using System.Net;
using System.Net.Http;
using System.Net.Security;

var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "gateway" }));

app.MapReverseProxy();

app.Run();
