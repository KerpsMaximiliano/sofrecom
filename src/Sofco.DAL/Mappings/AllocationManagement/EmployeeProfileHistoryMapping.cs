using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class EmployeeProfileHistoryMapping
    {
        public static void MapEmployeeProfileHistory(this ModelBuilder builder)
        {
            builder.Entity<EmployeeProfileHistory>().HasKey(_ => _.Id);
            builder.Entity<EmployeeProfileHistory>().Property(x => x.EmployeeNumber).HasMaxLength(50);
        }
    }
}
