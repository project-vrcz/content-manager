using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;

namespace VRChatContentManager.App.Shared.Controls;

public class AsyncImageLoader
{
    private static readonly ParametrizedLogger? Logger;

    static AsyncImageLoader()
    {
        SourceProperty.Changed.AddClassHandler<Control>(OnSourceChanged);
        Logger = Avalonia.Logging.Logger.TryGet(LogEventLevel.Error, nameof(AsyncImageLoader));
    }

    private static async void OnSourceChanged(Control control, AvaloniaPropertyChangedEventArgs args)
    {
        if (control is not Image)
            return;

        var (oldValue, newValue) = args.GetOldAndNewValue<string?>();
        if (oldValue == newValue)
            return;

        SetIsLoading(control, true);

        Bitmap? bitmap = null;
        try
        {
            if (newValue is not null)
            {
                control.Measure(Size.Infinity);
                control.Arrange(new Rect(control.DesiredSize));

                var height = GetHeight(control);
                var width = GetWidth(control);

                if (control.GetTransformedBounds() is { } bounds)
                {
                    if (bounds.Bounds.Height > 0)
                        height = (int)bounds.Bounds.Height;

                    if (bounds.Bounds.Width > 0)
                        width = (int)bounds.Bounds.Width;
                }

                bitmap = await AppWebImageLoader.Instance.ProvideImageAsyncWithSize(newValue,
                    width > 0 ? width : null,
                    height > 0 ? height : null);
            }
        }
        catch (Exception e)
        {
            Logger?.Log(nameof(AsyncImageLoader), nameof(AsyncImageLoader) + " image resolution failed: {0}", e);
        }

        if (GetSource(control) != newValue) return;

        switch (control)
        {
            case Image image:
                image.Source = bitmap;
                break;
        }

        SetIsLoading(control, false);
    }

    public static readonly AttachedProperty<string?> SourceProperty =
        AvaloniaProperty.RegisterAttached<ImageBrush, string?>("Source", typeof(AsyncImageLoader));

    public static string? GetSource(Control element)
    {
        return element.GetValue(SourceProperty);
    }

    public static void SetSource(Control element, string? value)
    {
        element.SetValue(SourceProperty, value);
    }

    public static readonly AttachedProperty<bool> IsLoadingProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsLoading", typeof(AsyncImageLoader));

    public static bool GetIsLoading(Control element)
    {
        return element.GetValue(IsLoadingProperty);
    }

    private static void SetIsLoading(Control element, bool value)
    {
        element.SetValue(IsLoadingProperty, value);
    }

    public static readonly AttachedProperty<int> WidthProperty =
        AvaloniaProperty.RegisterAttached<Control, int>("Width", typeof(AsyncImageLoader));

    public static readonly AttachedProperty<int> HeightProperty =
        AvaloniaProperty.RegisterAttached<Control, int>("Height", typeof(AsyncImageLoader));

    public static int GetWidth(Control element)
    {
        return element.GetValue(WidthProperty);
    }

    public static void SetWidth(Control element, int value)
    {
        element.SetValue(WidthProperty, value);
    }

    public static int GetHeight(Control element)
    {
        return element.GetValue(WidthProperty);
    }

    public static void SetHeight(Control element, int value)
    {
        element.SetValue(HeightProperty, value);
    }
}