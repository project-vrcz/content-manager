using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using VRChatContentManager.App.ContentManagement.ViewModels.Dialogs.Data.Avatar;
using VRChatContentManager.App.Shared;
using VRChatContentManager.App.Shared.Services;
using VRChatContentManager.Core.Management.Models.Entity.Avatar;

namespace VRChatContentManager.App.ContentManagement.ViewModels.Data.List;

public sealed partial class AvatarListItemViewModel(
    AvatarContentEntity avatarContentEntity,
    [FromKeyedServices(ServicesKeys.ContentManagerWindows)]
    DialogService dialogService,
    EditAvatarLocalTagsDialogViewModelFactory editTagsDialogViewModelFactory)
{
    public string? ThumbnailUrl => avatarContentEntity.ThumbnailImageUrl;

    public string Id => avatarContentEntity.Id;
    public string Name => avatarContentEntity.Name;

    public AvaloniaList<string> LocalTags => new(avatarContentEntity.LocalTags.Select(tag => tag.Tag).ToArray());

    [RelayCommand]
    private async Task EditLocalTags()
    {
        var editTagsDialogViewModel = editTagsDialogViewModelFactory.Create(avatarContentEntity);
        await dialogService.ShowDialogAsync(editTagsDialogViewModel);
    }
}

public sealed class AvatarListItemViewModelFactory(
    [FromKeyedServices(ServicesKeys.ContentManagerWindows)]
    DialogService dialogService,
    EditAvatarLocalTagsDialogViewModelFactory editTagsDialogViewModelFactory)
{
    public AvatarListItemViewModel Create(AvatarContentEntity avatarContentEntity)
    {
        return new AvatarListItemViewModel(avatarContentEntity, dialogService, editTagsDialogViewModelFactory);
    }
}