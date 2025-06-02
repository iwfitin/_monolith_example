namespace DTO.Infrastructure;

public readonly record struct LogResponseDto
{
    public int StatusCode { get; init; }

    public string UserName { get; init; }

    public string BodyJson { get; init; }
}
