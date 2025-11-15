using System;
using System.Linq;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.App.Services;
using VRChatContentManager.App.ViewModels.ContentManager.Data.Navigation;
using VRChatContentManager.App.ViewModels.ContentManager.Data.Navigation.Avatar;
using VRChatContentManager.App.ViewModels.ContentManager.Pages;
using VRChatContentManager.App.ViewModels.Pages;

namespace VRChatContentManager.App.ViewModels.ContentManager;

public sealed partial class ContentManagerViewModel : ViewModelBase, INavigationHost
{
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

    public ContentManagerViewModel(
        [FromKeyedServices(ServicesKeys.ContentManagerWindows)]
        NavigationService navigationService,
        TreeNavigationItemViewModelFactory navigationItemFactory,
        AvatarRootNavigationItemViewModel avatarRootNavigationItemViewModel
    )
    {
        navigationService.Register(this);

        NavigationItems =
        [
            navigationItemFactory.Create<ContentManagerHomePageViewModel>("Welcome"),
            avatarRootNavigationItemViewModel
        ];
    }

    public void Navigate(PageViewModelBase pageViewModel)
    {
        CurrentPage = pageViewModel;
        OnPropertyChanged(nameof(CurrentSelectedNavigationItem));
    }
}