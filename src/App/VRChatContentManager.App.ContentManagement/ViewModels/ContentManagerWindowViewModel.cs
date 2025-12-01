using VRChatContentManager.App.ViewModels;

namespace VRChatContentManager.App.ContentManagement.ViewModels;

public class ContentManagerWindowViewModel(MainViewModel mainViewModel) : ViewModelBase
{
    public MainViewModel MainViewModel => mainViewModel;
}