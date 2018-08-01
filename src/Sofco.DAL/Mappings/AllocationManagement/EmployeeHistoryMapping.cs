using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class EmployeeHistoryMapping
    {
        public static void MapEmployeeHistory(this ModelBuilder builder)
        {
            builder.Entity<EmployeeHistory>().HasKey(_ => _.Id);
            builder.Entity<EmployeeHistory>().Property(x => x.EmployeeNumber).HasMaxLength(50);
            builder.Entity<EmployeeHistory>().Property(x => x.Name).HasMaxLength(100);
        }
    }
}
