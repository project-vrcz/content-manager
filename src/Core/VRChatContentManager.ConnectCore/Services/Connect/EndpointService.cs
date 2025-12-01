using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VRChatContentManager.ConnectCore.Extensions;
using VRChatContentManager.ConnectCore.Models.Api.V1;

namespace VRChatContentManager.ConnectCore.Services.Connect;

public sealed class EndpointService(ILogger<EndpointService> logger, IServiceProvider serviceProvider)
{
    private readonly Dictionary<EndpointInfo, Func<HttpContext, IServiceProvider, Task>> _handlers = [];

    public void Map(string method, string path, Func<HttpContext, IServiceProvider, Task> handler)
    {
        var key = new EndpointInfo(path, method.ToUpperInvariant());
        _handlers[key] = handler;
    }

    public async Task HandleAsync(HttpContext context)
    {
        var requestPath = context.Request.Path.ToString();
        var requestMethod = context.Request.Method.ToUpperInvariant();

        var key = new EndpointInfo(requestPath, requestMethod);
        if (_handlers.TryGetValue(key, out var handler))
        {
            try
            {
                await handler(context, serviceProvider);
                return;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error handling request {Method} {Path}", requestMethod, requestPath);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return;
            }
        }

        if (_handlers.Any(pair => pair.Key.Path == requestPath))
        {
            await context.Response.WriteProblemAsync(ApiV1ProblemType.Undocumented,
                StatusCodes.Status405MethodNotAllowed,
                "Method Not Allowed", "The method is not allowed for the requested Endpoint.");
            return;
        }

        await context.Response.WriteProblemAsync(ApiV1ProblemType.Undocumented, StatusCodes.Status404NotFound,
            "Not Found", "The requested Endpoint was not found on the server.");
    }

    private record EndpointInfo(string Path, string Method);
}