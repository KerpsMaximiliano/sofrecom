using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class PrepaidHealthMapping
    {
        public static void MapPrepaidHealth(this ModelBuilder builder)
        {
            builder.Entity<PrepaidHealth>().HasKey(_ => _.Id);
            builder.Entity<PrepaidHealth>().Property(x => x.Name).HasMaxLength(400);
        }
    }
}
