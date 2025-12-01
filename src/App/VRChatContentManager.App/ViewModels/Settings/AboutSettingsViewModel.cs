using System;
using VRChatContentManager.Core.Utils;

namespace VRChatContentManager.App.ViewModels.Settings;

public sealed class AboutSettingsViewModel : ViewModelBase
{
    public string AppVersion => AppVersionUtils.GetAppVersion();
    public string AppCommitHash => AppVersionUtils.GetAppCommitHash();
    public DateTimeOffset? AppBuildDate => AppVersionUtils.GetAppBuildDate();
}