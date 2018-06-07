using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.Reports;

namespace Sofco.DAL.Mappings.Reports
{
    public static class PurchaseOrderBalanceDetailViewMapping
    {
        public static void MapPurchaseOrderBalanceDetailView(this ModelBuilder builder)
        {
            builder.Entity<PurchaseOrderBalanceDetailView>().ToTable("PurchaseOrderBalanceDetailView");
        }
    }
}
