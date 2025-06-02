using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Extensions;

public static class SerializeExtension
{
    private static JsonSerializerOptions Options { get; }

    static SerializeExtension()
    {
        Options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    public static string ToJson<T>(this T x)
    {
        return JsonSerializer.Serialize(x, Options);
    }

    public static T FromJson<T>(this string x, T @default = default)
    {
        return x.IsNullOrEmpty()
            ? @default
            : JsonSerializer.Deserialize<T>(x, Options);
    }
}
