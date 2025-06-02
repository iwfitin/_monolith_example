using Common.Interfaces;

namespace DTO.Users;

public static class ClaimDto
{
    public sealed record ListOut
    {
        public string Type { get; init; }

        public string Value { get; init; }
    }

    public sealed record Default : IHasId<int>
    {
        public int Id { get; set; }

        public string ClaimType { get; init; }

        public string ClaimValue { get; init; }
    }

    public sealed record Role: IHasId<string>
    {
        public string Id { get; set; }
    }
}
