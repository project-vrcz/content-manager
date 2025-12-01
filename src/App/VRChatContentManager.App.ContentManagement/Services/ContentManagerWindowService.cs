using VRChatContentManager.App.ContentManagement.ViewModels;
using VRChatContentManager.App.ContentManagement.Views;

namespace VRChatContentManager.App.ContentManagement.Services;

public sealed class ContentManagerWindowService(ContentManagerWindowViewModel contentManagerWindowViewModel)
{
    private ContentManagerWindow? _contentManagerWindow;

    public void ShowContentManagerWindow()
    {
        _contentManagerWindow ??= new ContentManagerWindow
        {
            DataContext = contentManagerWindowViewModel
        };

        _contentManagerWindow.Show();
        _contentManagerWindow.Activate();
    }
}