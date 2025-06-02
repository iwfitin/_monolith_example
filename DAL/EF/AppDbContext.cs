using DAL.Entities.Files;
using DAL.Entities.Users;
using DAL.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.EF;

public sealed class AppDbContext : IdentityDbContext<AspNetUser, Role, string,
    UserClaim, UserRole, IdentityUserLogin<string>,
    RoleClaim, IdentityUserToken<string>>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Admin> Admins { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        builder.ConfigureUser()
            .ConfigureSavedFile();

        foreach (var x in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            x.DeleteBehavior = DeleteBehavior.ClientCascade;
    }
}
