using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class LicenseTypeMapping
    {
        public static void MapLicenseType(this ModelBuilder builder)
        {
            builder.Entity<LicenseType>().HasKey(_ => _.Id);
            builder.Entity<LicenseType>().Property(x => x.Description).HasMaxLength(300);
        }
    }
}
