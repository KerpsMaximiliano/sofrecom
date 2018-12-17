using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sofco.Domain.Models.Workflow;

namespace Sofco.DAL.Mappings.Workflow
{
    public static class UserSourceMapping
    {
        public static void MapUserSource(this ModelBuilder builder)
        {
            builder.Entity<UserSource>().HasKey(_ => _.Id);

            builder.Entity<UserSource>().Property(x => x.Code).HasMaxLength(50);

            builder.Entity<UserSource>().HasMany(x => x.WorkflowStateAccesses).WithOne(x => x.UserSource).HasForeignKey(x => x.UserSourceId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<UserSource>().HasMany(x => x.WorkflowReadAccesses).WithOne(x => x.UserSource).HasForeignKey(x => x.UserSourceId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<UserSource>().HasMany(x => x.WorkflowStateNotifiers).WithOne(x => x.UserSource).HasForeignKey(x => x.UserSourceId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
