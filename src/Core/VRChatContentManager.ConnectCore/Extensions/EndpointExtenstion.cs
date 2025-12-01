using VRChatContentManager.ConnectCore.Endpoints.V1;
using VRChatContentManager.ConnectCore.Services.Connect;

namespace VRChatContentManager.ConnectCore.Extensions;

public static class EndpointExtenstion
{
    public static EndpointService MapConnectService(this EndpointService endpointService)
    {
        endpointService.MapMetadataEndpoint();
        endpointService.MapAuthEndpoints();
        endpointService.MapFileEndpoints();
        endpointService.MapTaskEndpoint();
        
        return endpointService;
    }
}