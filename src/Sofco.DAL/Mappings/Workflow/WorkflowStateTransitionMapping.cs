using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sofco.Domain.Models.Workflow;

namespace Sofco.DAL.Mappings.Workflow
{
    public static class WorkflowStateTransitionMapping
    {
        public static void MapWorkflowStateTransition(this ModelBuilder builder)
        {
            builder.Entity<WorkflowStateTransition>().HasKey(_ => _.Id);

            builder.Entity<WorkflowStateTransition>().Property(x => x.NotificationCode).HasMaxLength(50);
            builder.Entity<WorkflowStateTransition>().Property(x => x.ValidationCode).HasMaxLength(50);
            builder.Entity<WorkflowStateTransition>().Property(x => x.NotificationCode).HasMaxLength(50);

            builder.Entity<WorkflowStateTransition>().HasOne(x => x.ActualWorkflowState).WithMany(x => x.ActualTransitions).HasForeignKey(x => x.ActualWorkflowStateId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkflowStateTransition>().HasOne(x => x.NextWorkflowState).WithMany(x => x.NextTransitions).HasForeignKey(x => x.NextWorkflowStateId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkflowStateTransition>().HasOne(x => x.Workflow).WithMany(x => x.Transitions).HasForeignKey(x => x.WorkflowId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkflowStateTransition>().HasOne(x => x.CreatedBy).WithMany(x => x.WorkflowStateTransitions).HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkflowStateTransition>().HasOne(x => x.ModifiedBy).WithMany(x => x.WorkflowStateTransitions2).HasForeignKey(x => x.ModifiedById).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WorkflowStateTransition>().HasMany(x => x.WorkflowStateAccesses).WithOne(x => x.WorkflowStateTransition).HasForeignKey(x => x.WorkflowStateTransitionId);
            builder.Entity<WorkflowStateTransition>().HasMany(x => x.WorkflowStateNotifiers).WithOne(x => x.WorkflowStateTransition).HasForeignKey(x => x.WorkflowStateTransitionId);
        }
    }
}
