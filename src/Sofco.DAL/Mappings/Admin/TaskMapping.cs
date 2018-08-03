using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Admin;

namespace Sofco.DAL.Mappings.Admin
{
    public static class TaskMapping
    {
        public static void MapTasks(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Task>().HasKey(_ => _.Id);
            builder.Entity<Task>().Property(_ => _.Description).HasMaxLength(50).IsRequired();

            builder.Entity<Task>().HasOne(x => x.Category).WithMany(x => x.Tasks).HasForeignKey(x => x.CategoryId);
        }
    }
}
