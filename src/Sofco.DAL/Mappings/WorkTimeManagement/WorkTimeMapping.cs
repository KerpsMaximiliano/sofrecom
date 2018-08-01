using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.DAL.Mappings.WorkTimeManagement
{
    public static class WorkTimeMapping
    {
        public static void MapWorkTime(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<WorkTime>().HasKey(_ => _.Id);

            builder.Entity<WorkTime>().Property(x => x.Source).HasMaxLength(50);
            builder.Entity<WorkTime>().Property(x => x.ApprovalComment).HasMaxLength(500);
            builder.Entity<WorkTime>().Property(x => x.UserComment).HasMaxLength(500);

            builder.Entity<WorkTime>().HasOne(x => x.Employee).WithMany(x => x.WorkTimes).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkTime>().HasOne(x => x.User).WithMany(x => x.WorkTimes1).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkTime>().HasOne(x => x.Task).WithMany(x => x.WorkTimes).HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkTime>().HasOne(x => x.Analytic).WithMany(x => x.WorkTimes).HasForeignKey(x => x.AnalyticId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
