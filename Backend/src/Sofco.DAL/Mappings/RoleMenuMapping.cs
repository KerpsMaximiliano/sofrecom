using Microsoft.EntityFrameworkCore;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Mapping
{
    public static class RoleMenuMapping
    {
        public static void MapRoleMenu(this ModelBuilder builder)
        {
            builder.Entity<RoleMenu>().HasKey(t => new { t.RoleId, t.MenuId });

            builder.Entity<RoleMenu>()
                 .HasOne(pt => pt.Role)
                 .WithMany(p => p.RoleMenu)
                 .HasForeignKey(pt => pt.RoleId);

            builder.Entity<RoleMenu>()
                .HasOne(pt => pt.Menu)
                .WithMany(t => t.RoleMenu)
                .HasForeignKey(pt => pt.MenuId);
        }
    }
}
