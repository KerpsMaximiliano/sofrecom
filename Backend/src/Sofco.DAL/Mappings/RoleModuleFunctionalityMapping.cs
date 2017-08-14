using Microsoft.EntityFrameworkCore;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Mapping
{
    public static class RoleModuleFunctionalityMapping
    {
        public static void MapRoleModuleFunctionality(this ModelBuilder builder)
        {
            builder.Entity<RoleModuleFunctionality>().HasKey(t => new { t.RoleId, t.ModuleId, t.FunctionalityId });

            builder.Entity<RoleModuleFunctionality>()
                 .HasOne(pt => pt.Role)
                 .WithMany(p => p.RoleModuleFunctionality)
                 .HasForeignKey(pt => pt.RoleId);

            builder.Entity<RoleModuleFunctionality>()
                 .HasOne(pt => pt.Module)
                 .WithMany(p => p.RoleModuleFunctionality)
                 .HasForeignKey(pt => pt.ModuleId);

            builder.Entity<RoleModuleFunctionality>()
                .HasOne(pt => pt.Functionality)
                .WithMany(t => t.RoleModuleFunctionality)
                .HasForeignKey(pt => pt.FunctionalityId);
        }
    }
}
