using System;
using CommunityToolkit.Mvvm.ComponentModel;
using VRChatContentManager.App.Shared.Services;
using VRChatContentManager.App.Shared.ViewModels;
using VRChatContentManager.App.Shared.ViewModels.Pages;
using VRChatContentManager.App.ViewModels.Pages;

namespace VRChatContentManager.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, INavigationHost, IAppWindow
{
    [ObservableProperty] public partial PageViewModelBase? CurrentPage { get; private set; }

    [ObservableProperty] public partial bool Pinned { get; private set; }

    private readonly NavigationService _navigationService;
    private readonly DialogService _dialogService;

    public string DialogHostId { get; } = "MainWindow-" + Guid.NewGuid().ToString("D");

    public MainWindowViewModel(NavigationService navigationService, DialogService dialogService,
        AppWindowService appWindowService)
    {
        _navigationService = navigationService;
        _dialogService = dialogService;

        _dialogService.SetDialogHostId(DialogHostId);

        _navigationService.Register(this);

        _navigationService.Navigate<BootstrapPageViewModel>();
        
        appWindowService.Register(this);
    }

    public void Navigate(PageViewModelBase pageViewModel)
    {
        CurrentPage = pageViewModel;
    }

    public void SetPin(bool isPinned) => Pinned = isPinned;
    public bool IsPinned() => Pinned;
}