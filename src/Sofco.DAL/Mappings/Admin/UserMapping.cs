using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sofco.Domain.Models.Admin;

namespace Sofco.DAL.Mappings.Admin
{
    public static class UserMapping
    {
        public static void MapUsers(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<User>().HasKey(_ => _.Id);
            builder.Entity<User>().Property(_ => _.Name).HasMaxLength(150).IsRequired();
            builder.Entity<User>().Property(_ => _.Email).HasMaxLength(150).IsRequired();
            builder.Entity<User>().Property(_ => _.UserName).HasMaxLength(150).IsRequired();
            builder.Entity<User>().Property(_ => _.ExternalManagerId).HasMaxLength(100);

            builder.Entity<User>().HasMany(x => x.Invoices).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            builder.Entity<User>().HasMany(x => x.Analytics2).WithOne(x => x.Manager).HasForeignKey(x => x.ManagerId);
            builder.Entity<User>().HasMany(x => x.Analytics3).WithOne(x => x.CommercialManager).HasForeignKey(x => x.CommercialManagerId);

            builder.Entity<User>().HasMany(x => x.Licenses).WithOne(x => x.Manager).HasForeignKey(x => x.ManagerId);
            builder.Entity<User>().HasMany(x => x.Employees).WithOne(x => x.Manager).HasForeignKey(x => x.ManagerId);
            builder.Entity<User>().HasMany(x => x.Advancements).WithOne(x => x.UserApplicant).HasForeignKey(x => x.UserApplicantId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<User>().HasIndex(x => new { x.UserName }).IsUnique();
        }
    }
}
