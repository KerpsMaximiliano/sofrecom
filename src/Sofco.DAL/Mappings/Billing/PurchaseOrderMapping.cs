using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.Billing;

namespace Sofco.DAL.Mappings.Billing
{
    public static class PurchaseOrderMapping
    {
        public static void MapPurchaseOrder(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<PurchaseOrder>().HasKey(_ => _.Id);
            builder.Entity<PurchaseOrder>().Property(_ => _.Title).HasMaxLength(150);
            builder.Entity<PurchaseOrder>().Property(_ => _.Area).HasMaxLength(150);
            builder.Entity<PurchaseOrder>().Property(_ => _.ClientExternalId).HasMaxLength(150);
            builder.Entity<PurchaseOrder>().Property(_ => _.ClientExternalName).HasMaxLength(150);
            builder.Entity<PurchaseOrder>().Property(_ => _.Number).HasMaxLength(20);
            builder.Entity<PurchaseOrder>().Property(_ => _.UpdateByUser).HasMaxLength(25);

            builder.Entity<PurchaseOrder>().HasOne(x => x.Manager).WithMany(x => x.PurchaseOrder1).HasForeignKey(x => x.ManagerId);
            builder.Entity<PurchaseOrder>().HasOne(x => x.CommercialManager).WithMany(x => x.PurchaseOrder2).HasForeignKey(x => x.CommercialManagerId);
            builder.Entity<PurchaseOrder>().HasOne(x => x.Analytic).WithMany(x => x.PurchaseOrders).HasForeignKey(x => x.AnalyticId);
        }
    }
}
