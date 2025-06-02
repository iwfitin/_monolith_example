namespace DTO.Infrastructure;

public sealed record BadRequestDto
{
    public IDictionary<string, IEnumerable<string>> Errors { get; init; }

    public int? DevCode { get; init; }
}
