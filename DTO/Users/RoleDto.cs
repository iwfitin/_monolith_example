using System.ComponentModel.DataAnnotations;
using Common.Interfaces;

namespace DTO.Users;

public static class RoleDto
{
    public sealed record AddIn
    {
        [MaxLength(256)]
        public string Name { get; init; }
    }

    public sealed record ListOut : IHasId<string>, IIsDeletable
    {
        public string Id { get; set; }

        public string Name { get; init; }

        public bool IsDeletable { get; init; }
    }

    public sealed record EditIn : IHasId<string>
    {
        [Required]
        public string Id { get; set; }

        [MaxLength(256)]
        public string Name { get; init; }
    }

    public sealed record ByIdOut : IHasId<string>
    {
        public string Id { get; set; }

        public string Name { get; init; }
    }
}
