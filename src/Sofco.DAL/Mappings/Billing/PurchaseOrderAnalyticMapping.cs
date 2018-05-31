using Microsoft.EntityFrameworkCore;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Mappings.Billing
{
    public static class PurchaseOrderAnalyticMapping
    {
        public static void MapPurchaseOrderAnalytic(this ModelBuilder builder)
        {
            builder.Entity<PurchaseOrderAnalytic>().HasKey(t => new { t.PurchaseOrderId, t.AnalyticId });

            builder.Entity<PurchaseOrderAnalytic>()
                .HasOne(pt => pt.PurchaseOrder)
                .WithMany(p => p.PurchaseOrderAnalytics)
                .HasForeignKey(pt => pt.PurchaseOrderId);

            builder.Entity<PurchaseOrderAnalytic>()
                .HasOne(pt => pt.Analytic)
                .WithMany(t => t.PurchaseOrderAnalytics)
                .HasForeignKey(pt => pt.AnalyticId);
        }
    }
}
