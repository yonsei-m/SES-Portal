using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace 社員_求人管理アプリ.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        return value
            .GetType()
            .GetMember(value.ToString())[0]
            .GetCustomAttribute<DisplayAttribute>()
            ?.Name
            ?? value.ToString();
    }
}