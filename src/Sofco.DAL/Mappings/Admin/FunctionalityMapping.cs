using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Admin;

namespace Sofco.DAL.Mappings.Admin
{
    public static class FunctionalityMapping
    {
        public static void MapFunctionalities(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Functionality>().HasKey(_ => _.Id);
            builder.Entity<Functionality>().Property(_ => _.Description).HasMaxLength(50).IsRequired();
            builder.Entity<Functionality>().Property(_ => _.Code).HasMaxLength(20).IsRequired();

            builder.Entity<Functionality>().HasOne(x => x.Module).WithMany(x => x.Functionalities).HasForeignKey(x => x.ModuleId);
        }
    }
}
