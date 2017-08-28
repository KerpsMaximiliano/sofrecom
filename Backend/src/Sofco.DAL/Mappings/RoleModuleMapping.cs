using Microsoft.EntityFrameworkCore;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Mappings
{
    public static class RoleModuleMapping
    {
        public static void MapRoleModule(this ModelBuilder builder)
        {
            builder.Entity<RoleModule>().HasKey(t => new { t.RoleId, t.ModuleId });

            builder.Entity<RoleModule>()
                 .HasOne(pt => pt.Role)
                 .WithMany(p => p.RoleModule)
                 .HasForeignKey(pt => pt.RoleId);

            builder.Entity<RoleModule>()
                 .HasOne(pt => pt.Module)
                 .WithMany(p => p.RoleModule)
                 .HasForeignKey(pt => pt.ModuleId);
        }
    }
}
