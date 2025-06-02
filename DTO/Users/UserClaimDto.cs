using System.ComponentModel.DataAnnotations;
using Common.Interfaces;

namespace DTO.Users;

public static class UserClaimDto
{
    public sealed record AddIn
    {
        [Required]
        [MaxLength(450)]
        public string UserId { get; init; }

        [Required]
        [MaxLength(256)]
        public string ClaimType { get; init; }

        [Required]
        [MaxLength(256)]
        public string ClaimValue { get; init; }
    }

    public sealed record ListOut : IHasId<int>, IIsDeletable
    {
        public int Id { get; set; }

        public string UserId { get; init; }

        public string ClaimType { get; init; }

        public string ClaimValue { get; init; }

        public bool IsDeletable { get; init; }

        public OutUser User { get; init; }
    }

    public sealed record OutUser
    {
        public string Id { get; init; }

        public string UserName { get; init; }
    }

    public sealed record EditIn : IHasId<int>
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; init; }

        [Required]
        [MaxLength(256)]
        public string ClaimType { get; init; }

        [Required]
        [MaxLength(256)]
        public string ClaimValue { get; init; }
    }

    public sealed record ByIdOut : IHasId<int>
    {
        public int Id { get; set; }

        public string UserId { get; init; }

        public string ClaimType { get; init; }

        public string ClaimValue { get; init; }

        public OutUser User { get; init; }
    }
}
