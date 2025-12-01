using Microsoft.Extensions.Logging;
using VRChatContentManager.Core.Services.VRChatApi.S3;

namespace VRChatContentManager.Core.Services.VRChatApi;

public sealed class VRChatApiClientFactory(
    ILogger<VRChatApiClient> logger,
    ConcurrentMultipartUploaderFactory uploaderFactory)
{
    public VRChatApiClient Create(HttpClient sessionClient)
    {
        return new VRChatApiClient(sessionClient, logger, uploaderFactory);
    }
}