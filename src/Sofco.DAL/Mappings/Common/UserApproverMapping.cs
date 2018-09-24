using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Common;

namespace Sofco.DAL.Mappings.Common
{
    public static class UserApproverMapping
    {
        public static void MapUserApprover(this ModelBuilder builder)
        {
            builder.Entity<UserApprover>().HasKey(_ => _.Id);
            builder.Entity<UserApprover>().Property(_ => _.AnalyticId).IsRequired();
            builder.Entity<UserApprover>().Property(_ => _.ApproverUserId).IsRequired();
            builder.Entity<UserApprover>().Property(_ => _.UserId).IsRequired();
            builder.Entity<UserApprover>().Property(_ => _.CreatedUser).HasMaxLength(50);
            builder.Entity<UserApprover>().Property(_ => _.ModifiedUser).HasMaxLength(50);
            builder.Entity<UserApprover>().HasIndex(x => new { x.AnalyticId, x.EmployeeId, x.ApproverUserId, x.Type }).IsUnique();
        }
    }
}
