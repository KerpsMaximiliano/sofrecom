using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models;

namespace Sofco.DAL.Mapping
{
    public static class UserMapping
    {
        public static void MapUsers(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<User>().HasKey(_ => _.Id);
            builder.Entity<User>().Property(_ => _.Name).HasMaxLength(50).IsRequired();
            builder.Entity<User>().Property(_ => _.Email).HasMaxLength(50).IsRequired();
            builder.Entity<User>().Property(_ => _.UserName).HasMaxLength(50).IsRequired();
        }
    }
}
