namespace Common.Extensions;

public static class EnumerableExtension
{
    public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> lst)
    {
        return !lst.IsNullOrEmpty();
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> lst)
    {
        return lst is null || !lst.Any();
    }
}
