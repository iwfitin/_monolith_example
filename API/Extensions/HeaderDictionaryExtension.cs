namespace API.Extensions;

internal static class HeaderDictionaryExtension
{
    internal static IDictionary<string, string[]> ToDict(this IHeaderDictionary dict)
    {
        return dict.ToDictionary(x => x.Key, x => x.Value.ToArray());
    }
}
