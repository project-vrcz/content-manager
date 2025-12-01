using System;
using VRChatContentManager.Core.Services.UserSession;

namespace VRChatContentManager.App.ViewModels.Data.PublishTasks;

public sealed class InvalidSessionTaskManagerViewModel(Exception exception, UserSessionService userSessionService)
    : ViewModelBase, IPublishTaskManagerViewModel
{
    public Exception Exception => exception;
    public string ExceptionMessage => exception.Message;
    public string ExceptionString => exception.ToString();

    public string DisplayName => userSessionService.CurrentUser?.DisplayName ?? userSessionService.UserNameOrEmail;
}