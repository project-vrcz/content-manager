using VRChatContentManager.ConnectCore.Services.Connect.Metadata;
using VRChatContentManager.Core.Settings;
using VRChatContentManager.Core.Settings.Models;

namespace VRChatContentManager.Core.Services;

public sealed class ConnectMetadataProvider(IWritableOptions<AppSettings> appSettings) : IConnectMetadataProvider
{
    public string GetInstanceName() => appSettings.Value.ConnectInstanceName;

    public string GetImplementation() => "VRChatContentManager.Core";
    public string GetImplementationVersion() => "snapshot";
}