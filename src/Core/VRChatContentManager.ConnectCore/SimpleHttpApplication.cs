using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace VRChatContentManager.ConnectCore;

public class SimpleHttpApplication(Func<HttpContext, Task> handleRequest) : IHttpApplication<Context>
{
    public Context CreateContext(IFeatureCollection contextFeatures)
    {
        return new Context(contextFeatures);
    }

    public Task ProcessRequestAsync(Context context)
    {
        return handleRequest(new DefaultHttpContext(context.Features));
    }

    public void DisposeContext(Context context, Exception? exception)
    {
    }
}

public class Context(IFeatureCollection features)
{
    public readonly IFeatureCollection Features = features;
}