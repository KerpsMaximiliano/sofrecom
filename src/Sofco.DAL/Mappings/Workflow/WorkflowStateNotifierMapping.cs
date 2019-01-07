using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sofco.Domain.Models.Workflow;

namespace Sofco.DAL.Mappings.Workflow
{
    public static class WorkflowStateNotifierMapping
    {
        public static void MapWorkflowStateNotifier(this ModelBuilder builder)
        {
            builder.Entity<WorkflowStateNotifier>().HasKey(_ => _.Id);

            builder.Entity<WorkflowStateNotifier>().HasOne(x => x.WorkflowStateTransition).WithMany(x => x.WorkflowStateNotifiers).HasForeignKey(x => x.WorkflowStateTransitionId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkflowStateNotifier>().HasOne(x => x.UserSource).WithMany(x => x.WorkflowStateNotifiers).HasForeignKey(x => x.UserSourceId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkflowStateNotifier>().HasOne(x => x.CreatedBy).WithMany(x => x.WorkflowStateNotifiers).HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkflowStateNotifier>().HasOne(x => x.ModifiedBy).WithMany(x => x.WorkflowStateNotifiers2).HasForeignKey(x => x.ModifiedById).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
