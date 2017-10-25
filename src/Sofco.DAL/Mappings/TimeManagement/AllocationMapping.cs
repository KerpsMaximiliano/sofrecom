using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.TimeManagement;

namespace Sofco.DAL.Mappings.TimeManagement
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
        }
    }
}
