using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.TimeManagement;

namespace Sofco.DAL.Mappings.TimeManagement
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
