using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.TimeManagement;

namespace Sofco.DAL.Mappings.TimeManagement
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
