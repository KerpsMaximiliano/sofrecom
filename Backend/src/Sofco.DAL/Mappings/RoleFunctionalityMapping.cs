using Microsoft.EntityFrameworkCore;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Mapping
{
    public static class RoleFunctionalityMapping
    {
        public static void MapRoleFunctionality(this ModelBuilder builder)
        {
            builder.Entity<RoleFunctionality>().HasKey(t => new { t.RoleId, t.FunctionalityId });

            builder.Entity<RoleFunctionality>()
                 .HasOne(pt => pt.Role)
                 .WithMany(p => p.RoleFunctionality)
                 .HasForeignKey(pt => pt.RoleId);

            builder.Entity<RoleFunctionality>()
                .HasOne(pt => pt.Functionality)
                .WithMany(t => t.RoleFunctionality)
                .HasForeignKey(pt => pt.FunctionalityId);
        }
    }
}
