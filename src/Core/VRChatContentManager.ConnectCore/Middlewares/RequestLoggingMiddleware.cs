using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace VRChatContentManager.ConnectCore.Middlewares;

public sealed class RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger) : MiddlewareBase
{
    public override async Task ExecuteAsync(HttpContext context, Func<Task> next)
    {
        logger.LogInformation("{ClientIp}:{ClientPort} {Method} {Path}{Query}",
            context.Connection.RemoteIpAddress,
            context.Connection.RemotePort,
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString);

        await next();
    }
}