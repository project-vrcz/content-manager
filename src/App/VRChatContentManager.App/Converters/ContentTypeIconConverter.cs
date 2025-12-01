using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Material.Icons;

namespace VRChatContentManager.App.Converters;

public sealed class ContentTypeIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string valueString)
            throw new ArgumentException("The target must be a string");
        
        return valueString switch
        {
            "world" => MaterialIconKind.Earth,
            "avatar" => MaterialIconKind.Person,
            _ => MaterialIconKind.QuestionMark
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}