using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.AllocationManagement;

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
        }
    }
}
