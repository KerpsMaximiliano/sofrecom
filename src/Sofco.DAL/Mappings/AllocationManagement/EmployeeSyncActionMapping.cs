using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class EmployeeSyncActionMapping
    {
        public static void MapEmployeeSyncAction(this ModelBuilder builder)
        {
            builder.Entity<EmployeeSyncAction>().HasKey(_ => _.Id);
            builder.Entity<EmployeeSyncAction>().Property(x => x.EmployeeNumber).HasMaxLength(50);
            builder.Entity<EmployeeSyncAction>().Property(x => x.Status).HasMaxLength(20);
        }
    }
}
