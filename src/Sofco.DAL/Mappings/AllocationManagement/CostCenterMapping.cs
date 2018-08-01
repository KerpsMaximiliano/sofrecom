using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class CostCenterMapping
    {
        public static void MapCostCenter(this ModelBuilder builder)
        {
            builder.Entity<CostCenter>().HasKey(_ => _.Id);
            builder.Entity<CostCenter>().Property(_ => _.Code).HasMaxLength(3);
            builder.Entity<CostCenter>().Property(_ => _.Letter).HasMaxLength(1);
            builder.Entity<CostCenter>().Property(_ => _.Description).HasMaxLength(250);

            builder.Entity<CostCenter>().HasMany(x => x.Analytics).WithOne(x => x.CostCenter).HasForeignKey(x => x.CostCenterId);
        }
    }
}
