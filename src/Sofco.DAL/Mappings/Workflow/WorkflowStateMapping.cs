using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sofco.Domain.Models.Workflow;

namespace Sofco.DAL.Mappings.Workflow
{
    public static class WorkflowStateMapping
    {
        public static void MapWorkflowState(this ModelBuilder builder)
        {
            builder.Entity<WorkflowState>().HasKey(_ => _.Id);

            builder.Entity<WorkflowState>().Property(x => x.Name).HasMaxLength(300);

            builder.Entity<WorkflowState>().HasMany(x => x.ActualTransitions).WithOne(x => x.ActualWorkflowState).HasForeignKey(x => x.ActualWorkflowStateId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkflowState>().HasMany(x => x.NextTransitions).WithOne(x => x.NextWorkflowState).HasForeignKey(x => x.NextWorkflowStateId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WorkflowState>().HasOne(x => x.CreatedBy).WithMany(x => x.WorkflowStates).HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkflowState>().HasOne(x => x.ModifiedBy).WithMany(x => x.WorkflowStates2).HasForeignKey(x => x.ModifiedById).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
