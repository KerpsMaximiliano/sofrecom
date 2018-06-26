﻿using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.Billing;

namespace Sofco.DAL.Mappings.Billing
{
    public static class PurchaseOrderMapping
    {
        public static void MapPurchaseOrder(this ModelBuilder builder)
        {
            builder.Entity<PurchaseOrder>().HasKey(_ => _.Id);
            builder.Entity<PurchaseOrder>().Property(_ => _.ClientExternalId).HasMaxLength(150);
            builder.Entity<PurchaseOrder>().Property(_ => _.Description).HasMaxLength(1000);
            builder.Entity<PurchaseOrder>().Property(_ => _.ClientExternalName).HasMaxLength(150);
            builder.Entity<PurchaseOrder>().Property(_ => _.Number).HasMaxLength(150);
            builder.Entity<PurchaseOrder>().Property(_ => _.UpdateByUser).HasMaxLength(25);

            builder.Entity<PurchaseOrder>().HasOne(x => x.File).WithMany().HasForeignKey(x => x.FileId);
            builder.Entity<PurchaseOrder>().HasOne(x => x.Area).WithMany(x => x.PurchaseOrders).HasForeignKey(x => x.AreaId);

            builder.Entity<PurchaseOrder>().HasMany(x => x.Solfacs).WithOne(x => x.PurchaseOrder).HasForeignKey(x => x.PurchaseOrderId);

            builder.Entity<PurchaseOrder>().HasMany(x => x.AmmountDetails).WithOne(x => x.PurchaseOrder).HasForeignKey(x => x.PurchaseOrderId);

            // Details
            builder.Entity<PurchaseOrderAmmountDetail>().HasKey(t => new { t.PurchaseOrderId, t.CurrencyId });
            builder.Entity<PurchaseOrderAmmountDetail>().HasOne(x => x.PurchaseOrder).WithMany(x => x.AmmountDetails).HasForeignKey(x => x.PurchaseOrderId);
            builder.Entity<PurchaseOrderAmmountDetail>().HasOne(x => x.Currency).WithMany(x => x.AmmountDetails).HasForeignKey(x => x.CurrencyId);
        }
    }
}
