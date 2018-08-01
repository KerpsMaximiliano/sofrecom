using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Admin;

namespace Sofco.DAL.Mappings.Admin
{
    public static class CategoryMapping
    {
        public static void MapCategory(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Category>().HasKey(_ => _.Id);
            builder.Entity<Category>().Property(_ => _.Description).HasMaxLength(100).IsRequired();

            builder.Entity<Category>().HasMany(x => x.Tasks).WithOne(x => x.Category).HasForeignKey(x => x.CategoryId);
        }
    }
}
