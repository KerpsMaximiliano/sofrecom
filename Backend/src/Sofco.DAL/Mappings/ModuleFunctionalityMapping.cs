using Microsoft.EntityFrameworkCore;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Mappings
{
    public static class ModuleFunctionalityMapping
    {
        public static void MapModuleFunctionality(this ModelBuilder builder)
        {
            builder.Entity<ModuleFunctionality>().HasKey(t => new { t.ModuleId, t.FunctionalityId });

            builder.Entity<ModuleFunctionality>()
                 .HasOne(pt => pt.Module)
                 .WithMany(p => p.ModuleFunctionality)
                 .HasForeignKey(pt => pt.ModuleId);

            builder.Entity<ModuleFunctionality>()
                 .HasOne(pt => pt.Functionality)
                 .WithMany(p => p.ModuleFunctionality)
                 .HasForeignKey(pt => pt.FunctionalityId);
        }
    }
}
