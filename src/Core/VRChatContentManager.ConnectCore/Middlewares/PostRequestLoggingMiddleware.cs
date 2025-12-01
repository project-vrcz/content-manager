using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace VRChatContentManager.ConnectCore.Middlewares;

public sealed class PostRequestLoggingMiddleware(ILogger<PostRequestLoggingMiddleware> logger) : MiddlewareBase
{
    public override Task ExecuteAsync(HttpContext context, Func<Task> next)
    {
        logger.LogInformation("{ClientIp}:{ClientPort} {Method} {Path}{Query} => {StatusCode}",
            context.Connection.RemoteIpAddress,
            context.Connection.RemotePort,
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            context.Response.StatusCode);
        
        return next();
    }
}