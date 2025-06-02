using Common.Enums;
using Common.Extensions;
using DAL.EF;
using DAL.Entities.Users;
using DTO;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace BLL.Users;

public sealed class AdminService : AspNetUserService<Admin>
{
    private PasswordHasher<AspNetUser> PasswordHasher { get; }

    public AdminService(AppDbContext context, PasswordHasher<AspNetUser> passwordHasher) : base(context)
    {
        PasswordHasher = passwordHasher;
    }

    public async Task<string> Add(AdminDto.Add dto)
    {
        if (dto.Password is not null)
            dto.PasswordHash = PasswordHasher.HashPassword(null, dto.Password);
        var cfg = Build();
        cfg.NewConfig<AdminDto.Add, Admin>()
            .Inherits<AdminDto.IUser, Admin>();

        return (await Add(dto, cfg)).Id;
    }

    private TypeAdapterConfig Build()
    {
        var cfg = new TypeAdapterConfig();
        cfg.NewConfig<AdminDto.IUser, Admin>()
            .IgnoreIf((s, _) => s.PasswordHash == null, x => x.SecurityStamp, x => x.PasswordHash);

        return cfg;
    }

    protected override async Task BeforeAdd(Admin entity)
    {
        await base.BeforeAdd(entity);
        entity.Roles = new List<UserRole>
        {
            new()
            {
                RoleId = RoleType.Admin.Id(),
            }
        };
    }

    public async Task Edit(AdminDto.Edit dto)
    {
        if (dto.Password is not null)
            dto.PasswordHash = PasswordHasher.HashPassword(null, dto.Password);
        var cfg = Build();
        cfg.NewConfig<AdminDto.Edit, Admin>()
            .Inherits<AdminDto.IUser, Admin>();

        await Edit(dto, cfg);
    }
}
