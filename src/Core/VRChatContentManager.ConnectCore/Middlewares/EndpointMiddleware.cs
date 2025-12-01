using Microsoft.AspNetCore.Http;
using VRChatContentManager.ConnectCore.Services;
using VRChatContentManager.ConnectCore.Services.Connect;

namespace VRChatContentManager.ConnectCore.Middlewares;

public sealed class EndpointMiddleware(EndpointService endpointService) : MiddlewareBase
{
    public override async Task ExecuteAsync(HttpContext context, Func<Task> next)
    {
        await endpointService.HandleAsync(context);
        
        await next();
    }
}