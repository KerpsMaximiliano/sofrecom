using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class EmployeeEndNotificationMapping
    {
        public static void MapEmployeeEndNotification(this ModelBuilder builder)
        {
            builder.Entity<EmployeeEndNotification>().HasKey(t => t.Id);
            builder.Entity<EmployeeEndNotification>().Property(x => x.Recipients).HasMaxLength(500);
        }
    }
}
