using System.Text.RegularExpressions;

namespace Common.Extensions;

public static class StringExtension
{
    public static string ToCapitalize(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        return $"{str[..1].ToUpper()}{str[1..].ToLower()}";
    }

    public static string ToCamelCase(this string str)
    {
        return string.IsNullOrEmpty(str)
            ? str
            : $"{str[..1].ToLower()}{str[1..]}";
    }

    public static string CutExceptionCode(this string str)
    {
        var re = new Regex(@"(?s)^(\d+)\.\s?(.+?)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        var match = re.Match(str);

        return match.Success ? match.Groups[2].Value : str;
    }

    public static int? ExceptionCode(this string str)
    {
        var re = new Regex(@"(?s)^(\d+)\.\s?(.+?)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        var match = re.Match(str);

        return match.Success
            ? Convert.ToInt32(match.Groups[1].Value)
            : null;
    }

    public static bool IsBase64Str(this string str)
    {
        var buffer = new byte[(str.Length * 3 + 3) / 4 - (str.Length > 0 && str[^1] == '='
            ? str.Length > 1 && str[^2] == '='
                ? 2 : 1 : 0)];

        return Convert.TryFromBase64String(str, buffer, out _);
    }

    public static bool IsNotNullOrWhiteSpace(this string str)
    {
        return !str.IsNullOrWhiteSpace();
    }

    public static bool IsNullOrWhiteSpace(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }
}
