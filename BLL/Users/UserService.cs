using Common.Enums;
using Common.Extensions;
using DAL.EF;
using DAL.Entities.Users;
using DTO;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace BLL.Users;

public sealed class UserService : AspNetUserService<User>
{
    private PasswordHasher<AspNetUser> PasswordHasher { get; }

    public UserService(AppDbContext context, PasswordHasher<AspNetUser> passwordHasher) : base(context)
    {
        PasswordHasher = passwordHasher;
    }

    public async Task<string> Add(UserDto.Add dto)
    {
        if (dto.Password is not null)
            dto.PasswordHash = PasswordHasher.HashPassword(null, dto.Password);
        var cfg = Build();
        cfg.NewConfig<UserDto.Add, User>()
            .Inherits<UserDto.IUser, User>();

        return (await Add(dto, cfg)).Id;
    }

    private TypeAdapterConfig Build()
    {
        var cfg = new TypeAdapterConfig();
        cfg.NewConfig<UserDto.IUser, User>()
            .IgnoreIf((s, _) => s.PasswordHash == null, x => x.SecurityStamp, x => x.PasswordHash);

        return cfg;
    }

    protected override async Task BeforeAdd(User entity)
    {
        await base.BeforeAdd(entity);
        entity.Roles = new List<UserRole>
        {
            new()
            {
                RoleId = RoleType.User.Id(),
            }
        };
    }

    public async Task Edit(UserDto.Edit dto)
    {
        if (dto.Password is not null)
            dto.PasswordHash = PasswordHasher.HashPassword(null, dto.Password);
        var cfg = Build();
        cfg.NewConfig<UserDto.Edit, User>()
            .Inherits<UserDto.IUser, User>();

        await Edit(dto, cfg);
    }
}
