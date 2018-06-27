using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.Admin;

namespace Sofco.DAL.Mappings.Admin
{
    public static class GroupMapping
    {
        public static void MapGroups(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Group>().HasKey(_ => _.Id);
            builder.Entity<Group>().Property(_ => _.Description).HasMaxLength(200).IsRequired();
            builder.Entity<Group>().Property(_ => _.Email).HasMaxLength(100).IsRequired();
            builder.Entity<Group>().Property(_ => _.Code).HasMaxLength(100);

            builder.Entity<Group>().HasOne(x => x.Role).WithMany(x => x.Groups).HasForeignKey(x => x.RoleId);
        }
    }
}
