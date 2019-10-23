using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class AllocationMapping
    {
        public static void MapAllocation(this ModelBuilder builder)
        {
            builder.Entity<Allocation>().HasKey(t => t.Id);

            builder.Entity<Allocation>()
                 .HasOne(pt => pt.Analytic)
                 .WithMany(p => p.Allocations)
                 .HasForeignKey(pt => pt.AnalyticId);

            builder.Entity<Allocation>()
                 .HasOne(pt => pt.Employee)
                 .WithMany(p => p.Allocations)
                 .HasForeignKey(pt => pt.EmployeeId);

            builder.Entity<ReportPowerBi>().HasKey(x => x.Id);
            builder.Entity<ReportPowerBi>().Property(x => x.Resource).HasMaxLength(200);
            builder.Entity<ReportPowerBi>().Property(x => x.Manager).HasMaxLength(200);
            builder.Entity<ReportPowerBi>().Property(x => x.Profile).HasMaxLength(200);
            builder.Entity<ReportPowerBi>().Property(x => x.Seniority).HasMaxLength(200);
            builder.Entity<ReportPowerBi>().Property(x => x.Technology).HasMaxLength(200);
        }
    }
}
