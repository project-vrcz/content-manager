using System.Net.Http.Json;
using VRChatContentManager.Core.Models.VRChatApi;
using VRChatContentManager.Core.Models.VRChatApi.Rest;
using VRChatContentManager.Core.Models.VRChatApi.Rest.Worlds;

namespace VRChatContentManager.Core.Services.VRChatApi;

public sealed partial class VRChatApiClient
{
    public async ValueTask<VRChatApiWorld> GetWorldAsync(string worldId)
    {
        var response = await httpClient.GetAsync($"worlds/{worldId}");

        await HandleErrorResponseAsync(response);

        var world = await response.Content.ReadFromJsonAsync(ApiJsonContext.Default.VRChatApiWorld);
        if (world is null)
            throw new UnexpectedApiBehaviourException("The API returned a null world object.");

        return world;
    }

    public async ValueTask<VRChatApiWorld> CreateWorldVersionAsync(string worldId,
        CreateWorldVersionRequest createRequest, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request = new HttpRequestMessage(HttpMethod.Put, $"worlds/{worldId}")
        {
            Content = JsonContent.Create(createRequest, ApiJsonContext.Default.CreateWorldVersionRequest)
        };

        var response = await httpClient.SendAsync(request, cancellationToken);

        await HandleErrorResponseAsync(response);

        var world = await response.Content.ReadFromJsonAsync(ApiJsonContext.Default.VRChatApiWorld, cancellationToken);
        if (world is null)
            throw new UnexpectedApiBehaviourException("The API returned a null world object.");

        return world;
    }

    public async ValueTask<VRChatApiWorld> CreateWorldAsync(
        CreateWorldRequest createRequest,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request = new HttpRequestMessage(HttpMethod.Post, "worlds")
        {
            Content = JsonContent.Create(createRequest, ApiJsonContext.Default.CreateWorldRequest)
        };

        var response = await httpClient.SendAsync(request, cancellationToken);

        await HandleErrorResponseAsync(response);

        var world = await response.Content.ReadFromJsonAsync(ApiJsonContext.Default.VRChatApiWorld, cancellationToken);
        if (world is null)
            throw new UnexpectedApiBehaviourException("The API returned a null world object.");

        return world;
    }
}