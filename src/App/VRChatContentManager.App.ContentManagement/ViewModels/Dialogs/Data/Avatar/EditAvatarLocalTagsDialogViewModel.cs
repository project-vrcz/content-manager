using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.App.Shared.ViewModels.Dialogs;
using VRChatContentManager.Core.Management.Models.Entity.Avatar;
using VRChatContentManager.Core.Management.Services;

namespace VRChatContentManager.App.ContentManagement.ViewModels.Dialogs.Data.Avatar;

public sealed partial class EditAvatarLocalTagsDialogViewModel(
    AvatarContentEntity avatarContentEntity,
    AvatarContentManagementService contentManagementService) : DialogViewModelBase
{
    public AvatarContentEntity AvatarContentEntity => avatarContentEntity;

    public AvaloniaList<string> LocalTags { get; } =
        new(avatarContentEntity.LocalTags.Select(tag => tag.Tag).ToArray());

    [ObservableProperty] public partial string TagToAdd { get; set; } = string.Empty;

    public AvaloniaList<string> AllLocalTags { get; } = [];

    [RelayCommand]
    private async Task Load()
    {
        AllLocalTags.Clear();

        var tags = await contentManagementService.GetAllTagsAsync();

        AllLocalTags.AddRange(tags.Select(tag => tag.Tag).ToArray());
    }

    [RelayCommand]
    private void AddTag()
    {
        if (!string.IsNullOrWhiteSpace(TagToAdd) && !LocalTags.Contains(TagToAdd))
        {
            LocalTags.Add(TagToAdd);
            TagToAdd = string.Empty;
        }
    }

    [RelayCommand]
    private async Task Complete()
    {
        AvatarContentEntity.LocalTags.Clear();
        var tags = await contentManagementService.GetAllTagsAsync();
        foreach (var tag in LocalTags)
        {
            var existingTag = tags.FirstOrDefault(t => t.Tag == tag);
            if (existingTag is not null)
            {
                AvatarContentEntity.LocalTags.Add(existingTag);
            }
            else
            {
                var newTag = await contentManagementService.AddTagAsync(tag);
                AvatarContentEntity.LocalTags.Add(newTag);
            }
        }

        await contentManagementService.UpdateAvatarAsync(AvatarContentEntity.Id, avatar =>
        {
            avatar.LocalTags = AvatarContentEntity.LocalTags;
        });

        RequestClose();
    }
}

public sealed class EditAvatarLocalTagsDialogViewModelFactory(AvatarContentManagementService contentManagementService)
{
    public EditAvatarLocalTagsDialogViewModel Create(AvatarContentEntity avatarContentEntity)
    {
        return new EditAvatarLocalTagsDialogViewModel(avatarContentEntity, contentManagementService);
    }
}