using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models;
using Sofco.Model.Models.Admin;

namespace Sofco.DAL.Mappings
{
    public static class FunctionalityMapping
    {
        public static void MapFunctionalities(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Functionality>().HasKey(_ => _.Id);
            builder.Entity<Functionality>().Property(_ => _.Description).HasMaxLength(50).IsRequired();
            builder.Entity<Functionality>().Property(_ => _.Code).HasMaxLength(5).IsRequired();
        }
    }
}
