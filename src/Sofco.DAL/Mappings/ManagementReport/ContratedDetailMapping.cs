using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.ManagementReport;

namespace Sofco.DAL.Mappings.ManagementReport
{
    public static class ContratedDetailMapping
    {
        public static void MapContratedDetail(this ModelBuilder builder)
        {
            builder.Entity<ContratedDetail>().HasKey(x => x.Id);

            builder.Entity<ContratedDetail>().Property(x => x.Name).HasMaxLength(250);

            builder.Entity<ContratedDetail>().HasOne(x => x.CostDetail).WithMany(x => x.ContratedDetails).HasForeignKey(x => x.CostDetailId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
