using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.Billing;

namespace Sofco.DAL.Mappings.Billing
{
    public static class InvoiceMapping
    {
        public static void MapInvoice(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Invoice>().HasKey(_ => _.Id);
            builder.Entity<Invoice>().Property(_ => _.AccountName).HasMaxLength(100);
            builder.Entity<Invoice>().Property(_ => _.Address).HasMaxLength(100);
            builder.Entity<Invoice>().Property(_ => _.Zipcode).HasMaxLength(50);
            builder.Entity<Invoice>().Property(_ => _.City).HasMaxLength(50);
            builder.Entity<Invoice>().Property(_ => _.Province).HasMaxLength(100);
            builder.Entity<Invoice>().Property(_ => _.Country).HasMaxLength(100);
            builder.Entity<Invoice>().Property(_ => _.Cuit).HasMaxLength(100);
            builder.Entity<Invoice>().Property(_ => _.Service).HasMaxLength(100);
            builder.Entity<Invoice>().Property(_ => _.Project).HasMaxLength(100);
            builder.Entity<Invoice>().Property(_ => _.Analytic).HasMaxLength(100);
            builder.Entity<Invoice>().HasMany(x => x.Details).WithOne(x => x.Invoice).HasForeignKey(x => x.InvoiceId);

            builder.Entity<InvoiceDetail>().HasKey(_ => _.Id);
            builder.Entity<InvoiceDetail>().Property(_ => _.Description).HasMaxLength(300);
            builder.Entity<InvoiceDetail>().HasOne(x => x.Invoice).WithMany(x => x.Details).HasForeignKey(x => x.InvoiceId);
        }
    }
}
