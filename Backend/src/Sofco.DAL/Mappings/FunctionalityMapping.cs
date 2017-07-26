using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models;

namespace Sofco.DAL.Mapping
{
    public static class FunctionalityMapping
    {
        public static void MapFunctionalities(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Functionality>().HasKey(_ => _.Id);
            builder.Entity<Functionality>().Property(_ => _.Description).HasMaxLength(50);
        }
    }
}
