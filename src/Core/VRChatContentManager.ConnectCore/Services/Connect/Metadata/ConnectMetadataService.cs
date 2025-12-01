namespace VRChatContentManager.ConnectCore.Services.Connect.Metadata;

public sealed class ConnectMetadataService(IConnectMetadataProvider metadataProvider)
{
    public string GetInstanceName() => metadataProvider.GetInstanceName();

    public string GetImplementation() => metadataProvider.GetImplementation();
    public string GetImplementationVersion() => metadataProvider.GetImplementationVersion();

    public string GetApiVersion() => "1.0.0";
}