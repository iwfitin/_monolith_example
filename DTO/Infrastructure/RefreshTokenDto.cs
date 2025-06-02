namespace DTO.Infrastructure;

public sealed record RefreshTokenDto
{
    public string UserName { get; init; }

    public DateTime CreateDate { get; init; }

    public DateTime ExpireDate { get; init; }
}
