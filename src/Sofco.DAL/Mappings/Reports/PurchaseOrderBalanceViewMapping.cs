using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.Reports;

namespace Sofco.DAL.Mappings.Reports
{
    public static class PurchaseOrderBalanceViewMapping
    {
        public static void MapPurchaseOrderBalanceView(this ModelBuilder builder)
        {
            builder.Entity<PurchaseOrderBalanceView>().ToTable("PurchaseOrderBalanceView");
        }
    }
}
