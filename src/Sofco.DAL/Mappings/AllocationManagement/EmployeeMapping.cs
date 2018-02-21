using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class EmployeeMapping
    {
        public static void MapEmployee(this ModelBuilder builder)
        {
            builder.Entity<Employee>().HasKey(_ => _.Id);
            builder.Entity<Employee>().HasIndex(u => u.EmployeeNumber).IsUnique();
            builder.Entity<Employee>().Property(x => x.Name).HasMaxLength(100);
            builder.Entity<Employee>().Property(x => x.Profile).HasMaxLength(100);
            builder.Entity<Employee>().Property(x => x.Seniority).HasMaxLength(100);
            builder.Entity<Employee>().Property(x => x.Technology).HasMaxLength(300);
            builder.Entity<Employee>().Property(x => x.CreatedByUser).HasMaxLength(50);
            builder.Entity<Employee>().Property(x => x.Address).HasMaxLength(400);
            builder.Entity<Employee>().Property(x => x.Location).HasMaxLength(200);
            builder.Entity<Employee>().Property(x => x.Province).HasMaxLength(200);
            builder.Entity<Employee>().Property(x => x.Country).HasMaxLength(200);
            builder.Entity<Employee>().Property(x => x.OfficeAddress).HasMaxLength(400);
        }
    }
}
