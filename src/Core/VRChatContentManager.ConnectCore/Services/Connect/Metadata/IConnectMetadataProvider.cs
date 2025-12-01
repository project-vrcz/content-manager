namespace VRChatContentManager.ConnectCore.Services.Connect.Metadata;

public interface IConnectMetadataProvider
{
    string GetInstanceName();

    string GetImplementation();
    string GetImplementationVersion();
}