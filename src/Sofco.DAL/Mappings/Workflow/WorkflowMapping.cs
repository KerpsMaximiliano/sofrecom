using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.DAL.Mappings.Workflow
{
    public static class WorkflowMapping
    {
        public static void MapWorkflow(this ModelBuilder builder)
        {
            builder.Entity<Domain.Models.Workflow.Workflow>().HasKey(_ => _.Id);

            builder.Entity<Domain.Models.Workflow.Workflow>().Property(x => x.Description).HasMaxLength(200);
            builder.Entity<Domain.Models.Workflow.Workflow>().Property(x => x.Version).HasMaxLength(50);

            builder.Entity<Domain.Models.Workflow.Workflow>().HasOne(x => x.WorkflowType).WithMany(x => x.Workflows).HasForeignKey(x => x.WorkflowTypeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Domain.Models.Workflow.Workflow>().HasOne(x => x.CreatedBy).WithMany(x => x.Workflows).HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Domain.Models.Workflow.Workflow>().HasOne(x => x.ModifiedBy).WithMany(x => x.Workflows2).HasForeignKey(x => x.ModifiedById).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Domain.Models.Workflow.Workflow>().HasMany(x => x.Transitions).WithOne(x => x.Workflow).HasForeignKey(x => x.WorkflowId);
            builder.Entity<Domain.Models.Workflow.Workflow>().HasMany(x => x.WorkflowReadAccesses).WithOne(x => x.Workflow).HasForeignKey(x => x.WorkflowId);
        }
    }
}
