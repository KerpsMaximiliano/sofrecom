using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Utils;

namespace Sofco.DAL.Mappings.Rrhh
{
    public static class PrepaidMapping
    {
        public static void MapPrepaid(this ModelBuilder builder)
        {
            builder.Entity<Prepaid>().HasKey(_ => _.Id);
            builder.Entity<Prepaid>().Property(x => x.Text).HasMaxLength(250);
            builder.Entity<Prepaid>().Property(x => x.Code).HasMaxLength(100);

            builder.Entity<PrepaidImportedData>().HasKey(_ => _.Id);
            builder.Entity<PrepaidImportedData>().Property(x => x.Prepaid).HasMaxLength(100);
            builder.Entity<PrepaidImportedData>().Property(x => x.PrepaidPlan).HasMaxLength(100);
            builder.Entity<PrepaidImportedData>().Property(x => x.TigerPlan).HasMaxLength(100);
            builder.Entity<PrepaidImportedData>().Property(x => x.Comments).HasMaxLength(500);
            builder.Entity<PrepaidImportedData>().Property(x => x.Cuil).HasMaxLength(15);
            builder.Entity<PrepaidImportedData>().Property(x => x.Dni).HasMaxLength(15);
            builder.Entity<PrepaidImportedData>().Property(x => x.EmployeeName).HasMaxLength(100);
            builder.Entity<PrepaidImportedData>().Property(x => x.EmployeeNumber).HasMaxLength(15);
        }
    }
}
