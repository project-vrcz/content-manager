using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.App.ContentManagement.ViewModels.Data.Navigation;
using VRChatContentManager.App.ContentManagement.ViewModels.Data.Navigation.Avatar;
using VRChatContentManager.App.ContentManagement.ViewModels.Pages;
using VRChatContentManager.App.Shared;
using VRChatContentManager.App.Shared.Services;
using VRChatContentManager.App.Shared.ViewModels;
using VRChatContentManager.App.Shared.ViewModels.Pages;
using VRChatContentManager.App.ViewModels;

namespace VRChatContentManager.App.ContentManagement.ViewModels;

public sealed partial class MainViewModel : ViewModelBase, INavigationHost
{
    private readonly AvatarRootNavigationItemViewModel _avatarRootNavigationItemViewModel;

    public string DialogHostId { get; } = Guid.NewGuid().ToString("D");

    [ObservableProperty] public partial PageViewModelBase? CurrentPage { get; private set; }

    [ObservableProperty] public partial AvaloniaList<ITreeNavigationItemViewModel> NavigationItems { get; private set; }

    public ITreeNavigationItemViewModel? CurrentSelectedNavigationItem
    {
        get
        {
            if (CurrentPage is null)
                return null;

            return NavigationItems.FirstOrDefault(item => item.Match(CurrentPage));
        }
        set
        {
            if (value is null)
                return;

            if (CurrentPage is not null && value.Match(CurrentPage))
                return;

            value.NavigateCommand.Execute(null);
            OnPropertyChanged();
        }
    }

    public MainViewModel(
        [FromKeyedServices(ServicesKeys.ContentManagerWindows)]
        NavigationService navigationService,
        [FromKeyedServices(ServicesKeys.ContentManagerWindows)]
        DialogService dialogService,
        TreeNavigationItemViewModelFactory navigationItemFactory,
        AvatarRootNavigationItemViewModel avatarRootNavigationItemViewModel
    )
    {
        dialogService.SetDialogHostId(DialogHostId);

        _avatarRootNavigationItemViewModel = avatarRootNavigationItemViewModel;

        navigationService.Register(this);

        NavigationItems =
        [
            navigationItemFactory.Create<HomePageViewModel>("Welcome"),
            avatarRootNavigationItemViewModel
        ];
    }

    [RelayCommand]
    private async Task Load()
    {
        await _avatarRootNavigationItemViewModel.LoadChildrenAsync();
    }

    public void Navigate(PageViewModelBase pageViewModel)
    {
        CurrentPage = pageViewModel;
        OnPropertyChanged(nameof(CurrentSelectedNavigationItem));
    }
}