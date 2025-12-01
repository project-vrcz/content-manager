namespace VRChatContentManager.ConnectCore.Models.Api.V1.Responses.Meta;

public class ApiV1MetadataResponse
{
    public string InstanceName { get; set; } = "VRChatContentManager.LocalTest";
    
    public string Implementation { get; set; } = "VRChatContentManager.Connect";
    public string ImplementationVersion { get; set; } = "0.1.0";
    
    public string ApiVersion { get; set; } = "1.0.0";
}