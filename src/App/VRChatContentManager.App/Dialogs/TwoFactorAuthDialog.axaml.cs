using Avalonia.Controls;
using Avalonia.Input;
using VRChatContentManager.App.ViewModels.Dialogs;

namespace VRChatContentManager.App.Dialogs;

public partial class TwoFactorAuthDialog : UserControl
{
    public TwoFactorAuthDialog()
    {
        InitializeComponent();
    }

    private void Totp_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.NumPad0 && e.Key != Key.NumPad1 && e.Key != Key.NumPad2 && e.Key != Key.NumPad3 &&
            e.Key != Key.NumPad4 && e.Key != Key.NumPad5 && e.Key != Key.NumPad6 && e.Key != Key.NumPad7 &&
            e.Key != Key.NumPad8 && e.Key != Key.NumPad9 && e.Key != Key.D0 && e.Key != Key.D1 &&
            e.Key != Key.D2 && e.Key != Key.D3 && e.Key != Key.D4 && e.Key != Key.D5 && e.Key != Key.D6 &&
            e.Key != Key.D7 && e.Key != Key.D8 && e.Key != Key.D9 && e.Key != Key.Back)
        {
            e.Handled = true;
        }
    }

    private void Totp_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (DataContext is not TwoFactorAuthDialogViewModel viewModel)
            return;
        
        if (sender is not TextBox textBox)
            return;
        
        if (textBox.Text?.Length != 6)
            return;
        
        viewModel.VerifyCommand.Execute(null);
    }
}