using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.Admin;

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

            builder.Entity<User>().HasMany(x => x.Invoices).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            builder.Entity<User>().HasMany(x => x.Analytics1).WithOne(x => x.Director).HasForeignKey(x => x.DirectorId);
            builder.Entity<User>().HasMany(x => x.Analytics2).WithOne(x => x.Manager).HasForeignKey(x => x.ManagerId);
            builder.Entity<User>().HasMany(x => x.Analytics3).WithOne(x => x.CommercialManager).HasForeignKey(x => x.CommercialManagerId);
        }
    }
}
