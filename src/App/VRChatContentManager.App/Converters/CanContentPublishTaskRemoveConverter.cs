using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VRChatContentManager.Core.Models;

namespace VRChatContentManager.App.Converters;

public sealed class CanContentPublishTaskRemoveConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ContentPublishTaskStatus status)
            return false;

        return status is
            ContentPublishTaskStatus.Failed or
            ContentPublishTaskStatus.Canceled or
            ContentPublishTaskStatus.Completed;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}