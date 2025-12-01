using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRChatContentManager.Core.Settings;
using VRChatContentManager.Core.Settings.Models;

namespace VRChatContentManager.App.ViewModels.Settings;

public sealed partial class ConnectSettingsViewModel : ViewModelBase
{
    [ObservableProperty] public partial string ConnectInstanceName { get; set; }

    [ObservableProperty] public partial bool IsSettingsModified { get; private set; }

    private readonly IWritableOptions<AppSettings> _appSettings;

    public ConnectSettingsViewModel(IWritableOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;

        ConnectInstanceName = appSettings.Value.ConnectInstanceName;

        PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(IsSettingsModified))
                return;

            IsSettingsModified = true;
        };
    }

    [RelayCommand]
    private async Task ApplyConnectSettings()
    {
        await _appSettings.UpdateAsync(settings => { settings.ConnectInstanceName = ConnectInstanceName; });
        IsSettingsModified = false;
    }
}