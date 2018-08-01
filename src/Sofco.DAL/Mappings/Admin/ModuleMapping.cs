using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Admin;

namespace Sofco.DAL.Mappings.Admin
{
    public static class ModuleMapping
    {
        public static void MapModules(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Module>().HasKey(_ => _.Id);
            builder.Entity<Module>().Property(_ => _.Description).HasMaxLength(50).IsRequired();
            builder.Entity<Module>().Property(_ => _.Code).HasMaxLength(5).IsRequired();

            builder.Entity<Module>().HasMany(x => x.Functionalities).WithOne(x => x.Module).HasForeignKey(x => x.ModuleId);
        }
    }
}
