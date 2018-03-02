using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.Rrhh;

namespace Sofco.DAL.Mappings.Rrhh
{
    public static class LicenseMapping
    {
        public static void MapLicense(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<License>().HasKey(_ => _.Id);

            builder.Entity<License>().Property(x => x.Comments).HasMaxLength(200);
            builder.Entity<License>().Property(x => x.ExamDescription).HasMaxLength(200);

            builder.Entity<License>().HasOne(x => x.Sector).WithMany(x => x.Licenses).HasForeignKey(x => x.SectorId);
            builder.Entity<License>().HasOne(x => x.Manager).WithMany(x => x.Licenses).HasForeignKey(x => x.ManagerId);
            builder.Entity<License>().HasOne(x => x.Employee).WithMany(x => x.Licenses).HasForeignKey(x => x.EmployeeId);
            builder.Entity<License>().HasOne(x => x.Type).WithMany(x => x.Licenses).HasForeignKey(x => x.TypeId);
        }
    }
}
