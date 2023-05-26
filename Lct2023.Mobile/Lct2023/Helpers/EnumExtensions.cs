using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Lct2023.Helpers;

public static class AttributeExtensions
{
    public static string GetDescription<TEnum>(this TEnum type, string defaultValue = default)
        where TEnum : Enum
    {
        var attribute = type.GetType().GetField(type.ToString())?.GetCustomAttributes().OfType<DescriptionAttribute>().FirstOrDefault();

        return attribute?.Description ?? defaultValue;
    }
}
