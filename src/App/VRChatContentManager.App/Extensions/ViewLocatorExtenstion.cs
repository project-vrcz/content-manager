using VRChatContentManager.App.Dialogs;
using VRChatContentManager.App.Pages;
using VRChatContentManager.App.Pages.GettingStarted;
using VRChatContentManager.App.Pages.HomeTab;
using VRChatContentManager.App.Pages.Settings;
using VRChatContentManager.App.Shared;
using VRChatContentManager.App.ViewModels.Data;
using VRChatContentManager.App.ViewModels.Data.Connect;
using VRChatContentManager.App.ViewModels.Data.PublishTasks;
using VRChatContentManager.App.ViewModels.Dialogs;
using VRChatContentManager.App.ViewModels.Pages;
using VRChatContentManager.App.ViewModels.Pages.GettingStarted;
using VRChatContentManager.App.ViewModels.Pages.HomeTab;
using VRChatContentManager.App.ViewModels.Pages.Settings;
using VRChatContentManager.App.ViewModels.Settings;
using VRChatContentManager.App.Views.Data.Connect;
using VRChatContentManager.App.Views.Data.PublishTasks;
using VRChatContentManager.App.Views.Data.Settings;
using VRChatContentManager.App.Views.Settings;

namespace VRChatContentManager.App.Extensions;

public static class ViewLocatorExtenstion
{
    extension(ViewLocator)
    {
        public static void RegisterAppViews()
        {
            ViewLocator.Register<BootstrapPageViewModel, BootstrapPage>();

            ViewLocator.Register<HomePageViewModel, HomePage>();
            ViewLocator.Register<SettingsPageViewModel, SettingsPage>();

            // HomePage Tabs
            ViewLocator.Register<HomeTasksPageViewModel, HomeTasksPage>();
            ViewLocator.Register<HomeContentsPageViewModel, HomeContentsPage>();

            // Getting Started Pages
            ViewLocator.Register<GuideWelcomePageViewModel, GuideWelcomePage>();
            ViewLocator.Register<GuideSetupUnityPageViewModel, GuideSetupUnityPage>();
            ViewLocator.Register<GuideFinishSetupPageViewModel, GuideFinishPage>();

            // Settings Pages
            ViewLocator.Register<SettingsFixAccountPageViewModel, SettingsFixAccountPage>();
            ViewLocator.Register<AddAccountPageViewModel, AddAccountPage>();

            // Dialogs
            ViewLocator.Register<TwoFactorAuthDialogViewModel, TwoFactorAuthDialog>();
            ViewLocator.Register<RequestChallengeDialogViewModel, RequestChallengeDialog>();

            // Data
            ViewLocator.Register<PublishTaskManagerViewModel, PublishTaskManagerView>();
            ViewLocator.Register<InvalidSessionTaskManagerViewModel, InvalidSessionTaskManagerView>();
            ViewLocator.Register<PublishTaskViewModel, PublishTaskView>();

            ViewLocator.Register<RpcClientSessionViewModel, RpcClientSessionView>();
            ViewLocator.Register<UserSessionViewModel, UserSessionView>();

            // Settings Section
            ViewLocator.Register<ConnectSettingsViewModel, ConnectSettingsView>();
            ViewLocator.Register<AccountsSettingsViewModel, AccountsSettingsView>();
            ViewLocator.Register<SessionsSettingsViewModel, SessionsSettingsView>();
            ViewLocator.Register<AboutSettingsViewModel, AboutSettingsView>();
        }
    }
}