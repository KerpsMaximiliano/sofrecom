using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.DAL.Mappings.Rrhh
{
    public static class LicenseMapping
    {
        public static void MapLicense(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<License>().HasKey(_ => _.Id);

            builder.Entity<License>().Property(x => x.Comments).HasMaxLength(500);
            builder.Entity<License>().Property(x => x.ExamDescription).HasMaxLength(200);

            builder.Entity<License>().HasOne(x => x.Sector).WithMany(x => x.Licenses).HasForeignKey(x => x.SectorId);
            builder.Entity<License>().HasOne(x => x.Manager).WithMany(x => x.Licenses).HasForeignKey(x => x.ManagerId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<License>().HasOne(x => x.Employee).WithMany(x => x.Licenses).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<License>().HasOne(x => x.Type).WithMany(x => x.Licenses).HasForeignKey(x => x.TypeId);

            builder.Entity<License>().HasMany(x => x.Histories).WithOne(x => x.License).HasForeignKey(x => x.LicenseId);
        }
    }
}
