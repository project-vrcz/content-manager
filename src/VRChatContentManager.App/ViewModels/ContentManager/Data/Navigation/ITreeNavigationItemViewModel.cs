using System.ComponentModel;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.ViewModels.Pages;

namespace VRChatContentManager.App.ViewModels.ContentManager.Data.Navigation;

public interface ITreeNavigationItemViewModel : INotifyPropertyChanged, INotifyPropertyChanging
{
    string Name { get; }
    AvaloniaList<ITreeNavigationItemViewModel> Children { get; }

    bool Match(PageViewModelBase pageViewModel);

    IRelayCommand NavigateCommand { get; }
}