using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Billing;

namespace Sofco.DAL.Mappings.Billing
{
    public static class PurchaseOrderMapping
    {
        public static void MapPurchaseOrder(this ModelBuilder builder)
        {
            builder.Entity<PurchaseOrder>().HasKey(_ => _.Id);
            builder.Entity<PurchaseOrder>().Property(_ => _.AccountId).HasMaxLength(150);
            builder.Entity<PurchaseOrder>().Property(_ => _.Description).HasMaxLength(2000);
            builder.Entity<PurchaseOrder>().Property(_ => _.AccountName).HasMaxLength(150);
            builder.Entity<PurchaseOrder>().Property(_ => _.Number).HasMaxLength(150);
            builder.Entity<PurchaseOrder>().Property(_ => _.UpdateByUser).HasMaxLength(25);
            builder.Entity<PurchaseOrder>().Property(_ => _.FicheDeSignature).HasMaxLength(200);
            builder.Entity<PurchaseOrder>().Property(_ => _.PaymentForm).HasMaxLength(200);
            builder.Entity<PurchaseOrder>().Property(_ => _.Comments).HasMaxLength(2000);
            builder.Entity<PurchaseOrder>().Property(_ => _.Proposal).HasMaxLength(2000);
            builder.Entity<PurchaseOrder>().Property(_ => _.Title).HasMaxLength(1000);

            builder.Entity<PurchaseOrder>().HasOne(x => x.File).WithMany().HasForeignKey(x => x.FileId);
            builder.Entity<PurchaseOrder>().HasOne(x => x.Area).WithMany(x => x.PurchaseOrders).HasForeignKey(x => x.AreaId);

            builder.Entity<PurchaseOrder>().HasMany(x => x.Solfacs).WithOne(x => x.PurchaseOrder).HasForeignKey(x => x.PurchaseOrderId);
            builder.Entity<PurchaseOrder>().HasMany(x => x.Histories).WithOne(x => x.PurchaseOrder).HasForeignKey(x => x.PurchaseOrderId);

            builder.Entity<PurchaseOrder>().HasMany(x => x.AmmountDetails).WithOne(x => x.PurchaseOrder).HasForeignKey(x => x.PurchaseOrderId);

            // Details
            builder.Entity<PurchaseOrderAmmountDetail>().HasKey(t => new { t.PurchaseOrderId, t.CurrencyId });
            builder.Entity<PurchaseOrderAmmountDetail>().HasOne(x => x.PurchaseOrder).WithMany(x => x.AmmountDetails).HasForeignKey(x => x.PurchaseOrderId);
            builder.Entity<PurchaseOrderAmmountDetail>().HasOne(x => x.Currency).WithMany(x => x.AmmountDetails).HasForeignKey(x => x.CurrencyId);
        }
    }
}
