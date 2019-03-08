using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sofco.Domain.Models.Workflow;

namespace Sofco.DAL.Mappings.Workflow
{
    public static class WorkflowStateAccessMapping
    {
        public static void MapWorkflowStateAccess(this ModelBuilder builder)
        {
            builder.Entity<WorkflowStateAccess>().HasKey(_ => _.Id);

            builder.Entity<WorkflowStateAccess>().HasOne(x => x.WorkflowStateTransition).WithMany(x => x.WorkflowStateAccesses).HasForeignKey(x => x.WorkflowStateTransitionId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<WorkflowStateAccess>().HasOne(x => x.UserSource).WithMany(x => x.WorkflowStateAccesses).HasForeignKey(x => x.UserSourceId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkflowStateAccess>().HasOne(x => x.CreatedBy).WithMany(x => x.WorkflowStateAccesses).HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkflowStateAccess>().HasOne(x => x.ModifiedBy).WithMany(x => x.WorkflowStateAccesses2).HasForeignKey(x => x.ModifiedById).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
