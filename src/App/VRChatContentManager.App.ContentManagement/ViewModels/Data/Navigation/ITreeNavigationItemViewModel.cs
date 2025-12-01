using System.ComponentModel;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.Shared.ViewModels.Pages;

namespace VRChatContentManager.App.ContentManagement.ViewModels.Data.Navigation;

public interface ITreeNavigationItemViewModel : INotifyPropertyChanged, INotifyPropertyChanging
{
    string Name { get; }
    AvaloniaList<ITreeNavigationItemViewModel> Children { get; }

    bool Match(PageViewModelBase pageViewModel);

    IRelayCommand NavigateCommand { get; }
}