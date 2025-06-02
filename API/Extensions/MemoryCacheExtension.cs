using Microsoft.Extensions.Caching.Memory;

namespace API.Extensions;

public static class MemoryCacheExtension
{
    public static T GetOrThrow<T>(this IMemoryCache x, string key)
    {
        return x.Get<T>(key) ?? throw new ArgumentException($"2538. error key: '{key}' in cache");
    }
}
