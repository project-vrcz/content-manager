using VRChatContentManager.Core.Utils;

namespace VRChatContentManager.Core.Settings.Models;

public sealed class AppSettings
{
    public bool SkipFirstSetup { get; set; } = false;
    public string ConnectInstanceName { get; set; } = RandomWordsUtils.GetRandomWords();
}