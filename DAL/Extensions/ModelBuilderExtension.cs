using DAL.Entities.Files;
using DAL.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace DAL.Extensions;

internal static class ModelBuilderExtension
{
    internal static ModelBuilder ConfigureUser(this ModelBuilder builder)
    {
        builder.Entity<UserRole>()
            .HasOne(x => x.User)
            .WithMany(x => x.Roles)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.Entity<UserRole>()
            .HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId)
            .IsRequired();

        builder.Entity<UserClaim>()
            .HasOne(x => x.User)
            .WithMany(x => x.Claims)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.Entity<RoleClaim>()
            .HasOne(x => x.Role)
            .WithMany(x => x.Claims)
            .HasForeignKey(x => x.RoleId)
            .IsRequired();

        return builder;
    }
    
    internal static ModelBuilder ConfigureSavedFile(this ModelBuilder builder)
    {
        builder.Entity<SavedFile>()
            .Property(x => x.Name)
            .HasMaxLength(128);

        builder.Entity<SavedFile>()
            .Property(x => x.Path)
            .HasMaxLength(512);

        return builder;
    }
}
