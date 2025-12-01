namespace VRChatContentManager.Core;

public class InspectorHttpHandler(Func<Task> inspectorFunc) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        await inspectorFunc();
        return response;
    }
}