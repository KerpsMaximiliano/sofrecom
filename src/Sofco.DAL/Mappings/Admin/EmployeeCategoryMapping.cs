using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Relationships;

namespace Sofco.DAL.Mappings.Admin
{
    public static class EmployeeCategoryMapping
    {
        public static void MapEmployeeCategory(this ModelBuilder builder)
        {
            builder.Entity<EmployeeCategory>().HasKey(t => new { t.CategoryId, t.EmployeeId });

            builder.Entity<EmployeeCategory>()
                .HasOne(pt => pt.Category)
                .WithMany(p => p.EmployeeCategories)
                .HasForeignKey(pt => pt.CategoryId);

            builder.Entity<EmployeeCategory>()
                .HasOne(pt => pt.Employee)
                .WithMany(p => p.EmployeeCategories)
                .HasForeignKey(pt => pt.EmployeeId);
        }
    }
}
