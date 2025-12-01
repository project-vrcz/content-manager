using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using VRChatContentManager.App.ViewModels;

namespace VRChatContentManager.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
        ShowInTaskbar = true;
#endif
        
        Activate();

        Closing += (_, arg) =>
        {
            arg.Cancel = true;
            Hide();
        };

        Deactivated += (_, _) =>
        {
            if (DataContext is MainWindowViewModel { Pinned: true })
                return;
#if !DEBUG
            Hide();
#endif
        };

        // TransparencyLevelHint = [WindowTransparencyLevel.Mica];
    }

    private enum TaskBarLocation
    {
        Top,
        Bottom,
        Left,
        Right
    }

    private TaskBarLocation GetTaskBarLocation()
    {
        var taskBarOnTopOrBottom = Screens.Primary.WorkingArea.Width == Screens.Primary.Bounds.Width;
        if (taskBarOnTopOrBottom)
        {
            if (Screens.Primary.WorkingArea.TopLeft.Y > 0)
                return TaskBarLocation.Top;
        }
        else
        {
            return Screens.Primary.WorkingArea.TopLeft.X > 0 ? TaskBarLocation.Left : TaskBarLocation.Right;
        }

        return TaskBarLocation.Bottom;
    }

    private int GetTaskBarHeight()
    {
        var taskbarHeight = GetTaskBarLocation() switch
        {
            TaskBarLocation.Top => Screens.Primary.WorkingArea.Height,
            TaskBarLocation.Bottom => Screens.Primary.Bounds.Height - Screens.Primary.WorkingArea.Height,
            TaskBarLocation.Left => Screens.Primary.Bounds.Width - Screens.Primary.WorkingArea.Width,
            TaskBarLocation.Right => Screens.Primary.Bounds.Height - Screens.Primary.WorkingArea.Height,
            _ => throw new ArgumentOutOfRangeException()
        };

        return (int)(taskbarHeight / Screens.Primary.Scaling);
    }

    private void TopLevel_OnOpened(object? sender, EventArgs e)
    {
        UpdateWindowPosition();
    }

    private void UpdateWindowPosition()
    {
        var taskBarLocation = GetTaskBarLocation();
        var taskBarHeight = GetTaskBarHeight();

        var primaryScreen = Screens.Primary;
        var screenBoundsWithDpi = new PixelPoint(primaryScreen.Bounds.Width, primaryScreen.Bounds.Height);
        var windowBoundsWithDpi =
            PixelPoint.FromPoint(new Point((int)Bounds.Size.Width, (int)Bounds.Size.Height), primaryScreen.Scaling);
        switch (taskBarLocation)
        {
            case TaskBarLocation.Top:
                break;
            case TaskBarLocation.Bottom:
                Position = screenBoundsWithDpi - windowBoundsWithDpi -
                           PixelPoint.FromPoint(new Point(0, taskBarHeight), primaryScreen.Scaling) -
                           PixelPoint.FromPoint(new Point(4, 4), primaryScreen.Scaling);
                break;
            case TaskBarLocation.Left:
                break;
            case TaskBarLocation.Right:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}