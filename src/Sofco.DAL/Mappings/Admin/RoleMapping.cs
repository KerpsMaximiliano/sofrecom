using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.Admin;

namespace Sofco.DAL.Mappings.Admin
{
    public static class RoleMapping
    {
        public static void MapRoles(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Role>().HasKey(_ => _.Id);
            builder.Entity<Role>().Property(_ => _.Description).HasMaxLength(50).IsRequired();
        }
    }
}
