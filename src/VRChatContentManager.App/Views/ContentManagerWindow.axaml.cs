using Avalonia.Controls;
using VRChatContentManager.App.ViewModels.ContentManager;

namespace VRChatContentManager.App.Views;

public partial class ContentManagerWindow : Window
{
    public ContentManagerWindow(ContentManagerWindowViewModel viewModel)
    {
        DataContext = viewModel;

        InitializeComponent();
    }

    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }
}