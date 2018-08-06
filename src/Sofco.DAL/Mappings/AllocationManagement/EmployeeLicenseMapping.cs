using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class EmployeeLicenseMapping
    {
        public static void MapEmployeeLicense(this ModelBuilder builder)
        {
            builder.Entity<EmployeeLicense>().HasKey(_ => _.Id);
            builder.Entity<EmployeeLicense>().Property(x => x.EmployeeNumber).HasMaxLength(50);
            builder.Entity<EmployeeLicense>().Property(x => x.StartDate);
            builder.Entity<EmployeeLicense>().Property(x => x.EndDate);
            builder.Entity<EmployeeLicense>().Property(x => x.LicenseTypeNumber);
        }
    }
}
