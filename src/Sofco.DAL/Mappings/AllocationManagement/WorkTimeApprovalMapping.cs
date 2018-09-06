using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class WorkTimeApprovalMapping
    {
        public static void MapWorkTimeApproval(this ModelBuilder builder)
        {
            builder.Entity<WorkTimeApproval>().HasKey(_ => _.Id);
            builder.Entity<WorkTimeApproval>().Property(_ => _.AnalyticId).IsRequired();
            builder.Entity<WorkTimeApproval>().Property(_ => _.ApprovalUserId).IsRequired();
            builder.Entity<WorkTimeApproval>().Property(_ => _.UserId).IsRequired();
            builder.Entity<WorkTimeApproval>().Property(_ => _.CreatedUser).HasMaxLength(50);
            builder.Entity<WorkTimeApproval>().Property(_ => _.ModifiedUser).HasMaxLength(50);
            builder.Entity<WorkTimeApproval>().HasIndex(x => new { x.AnalyticId, x.EmployeeId, x.ApprovalUserId }).IsUnique();
            builder.Entity<WorkTimeApproval>().HasOne(x => x.Analytic)
                .WithMany(x => x.WorkTimeApprovals)
                .HasForeignKey(x => x.AnalyticId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
