using VRChatContentManager.App.ContentManagement.Dialogs.Data.Avatar;
using VRChatContentManager.App.ContentManagement.Pages;
using VRChatContentManager.App.ContentManagement.Pages.Avatar;
using VRChatContentManager.App.ContentManagement.ViewModels;
using VRChatContentManager.App.ContentManagement.ViewModels.Dialogs.Data.Avatar;
using VRChatContentManager.App.ContentManagement.ViewModels.Pages;
using VRChatContentManager.App.ContentManagement.ViewModels.Pages.Avatar;
using VRChatContentManager.App.ContentManagement.Views;
using VRChatContentManager.App.Shared;

namespace VRChatContentManager.App.ContentManagement.Extensions;

public static class ViewLocatorExtenstion
{
    extension(ViewLocator)
    {
        public static void RegisterContentManagementViews()
        {
            ViewLocator.Register<MainViewModel, MainView>();

            // Pages
            ViewLocator.Register<HomePageViewModel, ContentManagerHomePage>();

            ViewLocator.Register<AvatarRootPageViewModel, AvatarRootPage>();
            ViewLocator.Register<AvatarQueryFilterPageViewModel, AvatarQueryFilterPage>();
            
            // Dialogs
            ViewLocator.Register<EditAvatarLocalTagsDialogViewModel, EditAvatarLocalTagsDialog>();
        }
    }
}