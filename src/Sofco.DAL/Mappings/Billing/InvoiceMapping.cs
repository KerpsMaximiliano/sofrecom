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
            builder.Entity<Invoice>().Property(_ => _.ExcelFileName).HasMaxLength(150);
            builder.Entity<Invoice>().Property(_ => _.PdfFileName).HasMaxLength(150);
            builder.Entity<Invoice>().Property(_ => _.InvoiceNumber).HasMaxLength(50);

            builder.Entity<Invoice>().HasOne(x => x.User).WithMany(x => x.Invoices).HasForeignKey(x => x.UserId);
            builder.Entity<Invoice>().HasOne(x => x.Solfac).WithOne(x => x.Invoice).HasForeignKey<Solfac>(x => x.InvoiceId);
            builder.Entity<Invoice>().HasMany(x => x.Histories).WithOne(x => x.Invoice).HasForeignKey(x => x.InvoiceId);
        }
    }
}
