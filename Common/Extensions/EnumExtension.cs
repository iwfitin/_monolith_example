using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Common.Attributes;
using Common.Interfaces;

namespace Common.Extensions;

public static class EnumExtension
{
    public static string Description<T>(this T x) where T : struct, Enum
    {
        return x.GetAttr<T, DescriptionAttribute>(x => x.Description);
    }

    public static string Display<T>(this T x) where T : struct, Enum
    {
        return x.GetAttr<T, DisplayAttribute>(x => x.GetName());
    }

    public static string Id<T>(this T x) where T : struct, Enum
    {
        return x.GetAttr<T, EnumIdAttribute>(x => x.Id);
    }

    public static string GetAttr<T, TA>(this T x, Func<TA, string> name)
        where T : struct, Enum
        where TA : Attribute
    {
        var defaultDescription = x.ToString();
        var memberInfo = x.GetType().GetMember(defaultDescription);
        if (memberInfo.Length <= 0)
            return defaultDescription;

        var attrs = memberInfo[0].GetCustomAttributes(typeof(TA), false);

        return attrs.Length > 0
            ? name((TA)attrs[0])
            : defaultDescription;
    }

    public static byte ToByte<T>(this T x) where T : struct, Enum
    {
        return Convert.ToByte(x);
    }

    public static int ToInt<T>(this T x) where T : struct, Enum
    {
        return Convert.ToInt32(x);
    }

    public static string ToLowStr<T>(this T x) where T : struct, Enum
    {
        return x.ToString().ToLower();
    }

    public static string ToUpStr<T>(this T x) where T : struct, Enum
    {
        return x.ToString().ToUpper();
    }

    public static T FromStr<T>(this string str) where T : struct, Enum
    {
        foreach (var x in Enum.GetValues<T>())
            if (string.Equals(x.ToString(), str, StringComparison.InvariantCultureIgnoreCase))
                return x;

        throw new ArgumentException($"2525. unknown type {str}");
    }

    public static IEnumerable<T> Except<T>(this IEnumerable<T> lst, T x) where T : struct, Enum
    {
        return lst.Except(new[] { x, });
    }
    
    public static IEnumerable<TD> ToList<TE, TD>()
        where TE : struct, Enum
        where TD : IEnum, new()
    {
        return Enum.GetValues<TE>()
            .Select(x => new TD
            {
                Id = Convert.ToInt32(x),
                Name = x.Description(),
            });
    }

    public static bool IsCorrect<T>(this T x) where T : struct, Enum
    {
        return Enum.IsDefined(typeof(T), x);
    }
}
