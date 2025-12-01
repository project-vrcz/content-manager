using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VRChatContentManager.ConnectCore.Extensions;
using VRChatContentManager.ConnectCore.Middlewares;
using VRChatContentManager.ConnectCore.Models.Api.V1;

namespace VRChatContentManager.ConnectCore.Services.Connect;

public sealed class HttpServerService
{
    private readonly ILogger<HttpServerService> _logger;

    private readonly KestrelServer _kestrelServer;
    private readonly SimpleHttpApplication _simpleHttpApplication;

    private readonly List<MiddlewareBase> _preRequestMiddlewares = [];
    private readonly List<MiddlewareBase> _postRequestMiddlewares = [];

    public HttpServerService(ILoggerFactory loggerFactory, ILogger<HttpServerService> logger,
        RequestLoggingMiddleware requestLoggingMiddleware, EndpointMiddleware endpointMiddleware,
        PostRequestLoggingMiddleware postRequestLoggingMiddleware, JwtAuthMiddleware jwtAuthMiddleware)
    {
        _logger = logger;

        var kestrelServerOptions = new KestrelServerOptions();

        kestrelServerOptions.Limits.MaxRequestBodySize = null;
        kestrelServerOptions.ListenLocalhost(59328);

        var transportOptions = new SocketTransportOptions();
        var transportFactory = new SocketTransportFactory(
            new OptionsWrapper<SocketTransportOptions>(transportOptions), loggerFactory);

        _kestrelServer = new KestrelServer(
            new OptionsWrapper<KestrelServerOptions>(kestrelServerOptions), transportFactory, loggerFactory);
        _simpleHttpApplication = new SimpleHttpApplication(HandleRequestAsync);

        _preRequestMiddlewares.Add(requestLoggingMiddleware);
        _preRequestMiddlewares.Add(jwtAuthMiddleware);

        _postRequestMiddlewares.Add(endpointMiddleware);
        _postRequestMiddlewares.Add(postRequestLoggingMiddleware);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _kestrelServer.StartAsync(_simpleHttpApplication, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _kestrelServer.StopAsync(cancellationToken);
    }

    private async Task HandleRequestAsync(HttpContext httpContext)
    {
        try
        {
            var middlewares = new List<MiddlewareBase>();
            middlewares.AddRange(_preRequestMiddlewares);
            middlewares.AddRange(_postRequestMiddlewares);

            await RunMiddlewaresAsync(httpContext, middlewares);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling HTTP request");
            await httpContext.Response.WriteProblemAsync(ApiV1ProblemType.Undocumented,
                StatusCodes.Status500InternalServerError,
                "Internal Server Error", "An unexpected error occurred.");
        }
    }

    private async Task RunMiddlewaresAsync(HttpContext httpContext, List<MiddlewareBase> middlewares)
    {
        var index = 0;
        await Next();
        return;

        async Task Next()
        {
            if (index < middlewares.Count)
            {
                var current = middlewares[index];
                index++;
                await current.ExecuteAsync(httpContext, Next);
            }
        }
    }
}