namespace VRChatContentManager.App.Shared.ViewModels;

public interface IAppWindow
{
    void SetPin(bool isPinned);
    bool IsPinned();
}