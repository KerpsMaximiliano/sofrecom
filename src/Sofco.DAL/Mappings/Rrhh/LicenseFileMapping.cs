using Microsoft.EntityFrameworkCore;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Mappings.Rrhh
{
    public static class LicenseFileMapping
    {
        public static void MapLicenseFiles(this ModelBuilder builder)
        {
            builder.Entity<LicenseFile>().HasKey(t => new { t.LicenseId, t.FileId });

            builder.Entity<LicenseFile>()
                .HasOne(pt => pt.License)
                .WithMany(p => p.LicenseFiles)
                .HasForeignKey(pt => pt.LicenseId);

            builder.Entity<LicenseFile>()
                .HasOne(pt => pt.File)
                .WithMany(t => t.LicenseFiles)
                .HasForeignKey(pt => pt.FileId);
        }
    }
}
