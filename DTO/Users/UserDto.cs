using Common.Interfaces;

namespace DTO.Users;

public static class UserDto
{
    public sealed record AboutMeOut : IHasId<string>
    {
        public string Id { get; set; }

        public string VenueId { get; init; }

        public int? AreaId { get; init; }

        public string UserName { get; init; }

        public string Email { get; init; }

        public string PhoneNumber { get; init; }
        
        public IEnumerable<UserRole> Roles { get; init; }
    }

    public sealed record UserRole
    {
        public string UserId { get; init; }

        public string RoleId { get; init; }

        public Role Role { get; init; }
    }

    public sealed record Role
    {
        public string Name { get; init; }
    }
}
