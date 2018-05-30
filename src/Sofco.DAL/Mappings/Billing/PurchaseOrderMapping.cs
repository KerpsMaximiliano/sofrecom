using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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
            builder.Entity<PurchaseOrder>().Property(_ => _.ProjectId).HasMaxLength(150);
            builder.Entity<PurchaseOrder>().Property(_ => _.ClientExternalName).HasMaxLength(150);
            builder.Entity<PurchaseOrder>().Property(_ => _.Number).HasMaxLength(20);
            builder.Entity<PurchaseOrder>().Property(_ => _.UpdateByUser).HasMaxLength(25);

            builder.Entity<PurchaseOrder>().HasOne(x => x.Manager).WithMany(x => x.PurchaseOrder1).HasForeignKey(x => x.ManagerId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PurchaseOrder>().HasOne(x => x.CommercialManager).WithMany(x => x.PurchaseOrder2).HasForeignKey(x => x.CommercialManagerId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PurchaseOrder>().HasOne(x => x.File).WithMany().HasForeignKey(x => x.FileId);
        }
    }
}
