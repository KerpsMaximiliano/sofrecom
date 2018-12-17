using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sofco.Domain.Models.Workflow;

namespace Sofco.DAL.Mappings.Workflow
{
    public static class WorkflowTypeMapping
    {
        public static void MapWorkflowType(this ModelBuilder builder)
        {
            builder.Entity<WorkflowType>().HasKey(_ => _.Id);

            builder.Entity<WorkflowType>().Property(x => x.Name).HasMaxLength(300);
            builder.Entity<WorkflowType>().Property(x => x.Code).HasMaxLength(50);

            builder.Entity<WorkflowType>().HasOne(x => x.CreatedBy).WithMany(x => x.WorkflowTypes).HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkflowType>().HasOne(x => x.ModifiedBy).WithMany(x => x.WorkflowTypes2).HasForeignKey(x => x.ModifiedById).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
