using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sofco.Domain.Models.Workflow;

namespace Sofco.DAL.Mappings.Workflow
{
    public static class WorkflowReadAccessMapping
    {
        public static void MapWorkflowReadAccess(this ModelBuilder builder)
        {
            builder.Entity<WorkflowReadAccess>().HasKey(_ => _.Id);

            builder.Entity<WorkflowReadAccess>().HasOne(x => x.Workflow).WithMany(x => x.WorkflowReadAccesses).HasForeignKey(x => x.WorkflowId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkflowReadAccess>().HasOne(x => x.UserSource).WithMany(x => x.WorkflowReadAccesses).HasForeignKey(x => x.UserSourceId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkflowReadAccess>().HasOne(x => x.CreatedBy).WithMany(x => x.WorkflowReadAccesses).HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkflowReadAccess>().HasOne(x => x.ModifiedBy).WithMany(x => x.WorkflowReadAccesses2).HasForeignKey(x => x.ModifiedById).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
