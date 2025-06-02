namespace DTO.Infrastructure;

public readonly record struct LogRequestDto
{
    public string Method { get; init; }

    public string Path { get; init; }

    public string Query { get; init; }

    public string BodyJson { get; init; }

    public IDictionary<string, string[]> Headers { get; init; }
}
