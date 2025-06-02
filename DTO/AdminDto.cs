using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.Interfaces;

namespace DTO;

public static class AdminDto
{
    public interface IUser
    {
        string PasswordHash { get; set; }
    }

    public sealed record Add : IUser
    {
        [MaxLength(256)]
        public string Email { get; init; }

        [MaxLength(256)]
        public string Password { get; init; }

        [JsonIgnore]
        public string PasswordHash { get; set; }
    }

    public sealed record List
    {
        public string Id { get; init; }

        public string Email { get; init; }
    }

    public sealed record ByHasId : IHasId<string>
    {
        public string Id { get; set; }

        public string Email { get; init; }
    }

    public sealed record Edit : IHasId<string>, IUser
    {
        public string Id { get; set; }

        [MaxLength(256)]
        public string Email { get; init; }

        [MaxLength(256)]
        public string Password { get; init; }

        [JsonIgnore]
        public string PasswordHash { get; set; }
    }
}
