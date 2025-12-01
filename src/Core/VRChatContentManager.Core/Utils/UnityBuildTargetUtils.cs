namespace VRChatContentManager.Core.Utils;

public static class UnityBuildTargetUtils
{
    public static bool IsStandalonePlatform(string platform)
    {
        return platform is "standalonewindows" or "standalonewindows64" or "standalonelinux" or "standalonelinux64" or "standaloneosx";
    }
}