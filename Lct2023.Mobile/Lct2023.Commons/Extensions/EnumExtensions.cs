using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Lct2023.Commons.Extensions
{
    public static class EnumExtensions
    {
        public static string GetEnumMemberValue(this Enum enumValue) =>
            enumValue.GetType()?.GetField(enumValue.ToString())?.GetCustomAttribute<EnumMemberAttribute>()?.Value;
    }
}