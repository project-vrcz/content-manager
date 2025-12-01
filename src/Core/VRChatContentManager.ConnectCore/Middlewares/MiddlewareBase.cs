using Microsoft.AspNetCore.Http;

namespace VRChatContentManager.ConnectCore.Middlewares;

public abstract class MiddlewareBase
{
    public abstract Task ExecuteAsync(HttpContext context, Func<Task> next);
}