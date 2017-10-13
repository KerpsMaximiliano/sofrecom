using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.Billing;

namespace Sofco.DAL.Mappings.Billing
{
    public static class SolfacMapping
    {
        public static void MapSolfac(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Solfac>().HasKey(_ => _.Id);
            builder.Entity<Solfac>().Property(_ => _.ClientName).HasMaxLength(100);
            builder.Entity<Solfac>().Property(_ => _.BusinessName).HasMaxLength(100);
            builder.Entity<Solfac>().Property(_ => _.CelPhone).HasMaxLength(15);
            builder.Entity<Solfac>().Property(_ => _.ContractNumber).HasMaxLength(50);
            builder.Entity<Solfac>().Property(_ => _.Project).HasMaxLength(100);
            builder.Entity<Solfac>().Property(_ => _.ImputationNumber1).HasMaxLength(50);
            builder.Entity<Solfac>().Property(_ => _.ParticularSteps).HasMaxLength(500);
            builder.Entity<Solfac>().Property(_ => _.InvoiceCode).HasMaxLength(50).IsRequired(false);

            builder.Entity<Solfac>().HasOne(x => x.UserApplicant).WithMany(x => x.Solfacs).HasForeignKey(x => x.UserApplicantId);
            builder.Entity<Solfac>().HasOne(x => x.DocumentType).WithMany(x => x.Solfacs).HasForeignKey(x => x.DocumentTypeId);
            builder.Entity<Solfac>().HasOne(x => x.Currency).WithMany(x => x.Solfacs).HasForeignKey(x => x.CurrencyId);
            builder.Entity<Solfac>().HasOne(x => x.ImputationNumber).WithMany(x => x.Solfacs).HasForeignKey(x => x.ImputationNumber3Id);

            builder.Entity<Solfac>().HasMany(x => x.Hitos).WithOne(x => x.Solfac).HasForeignKey(x => x.SolfacId);
            builder.Entity<Solfac>().HasMany(x => x.Histories).WithOne(x => x.Solfac).HasForeignKey(x => x.SolfacId);
            builder.Entity<Solfac>().HasMany(x => x.Attachments).WithOne(x => x.Solfac).HasForeignKey(x => x.SolfacId);
            builder.Entity<Solfac>().HasMany(x => x.Invoices).WithOne(x => x.Solfac).HasForeignKey(x => x.SolfacId);

            builder.Entity<SolfacAttachment>().Property(_ => _.Name).HasMaxLength(200);
        }

        public static void MapHitos(this ModelBuilder builder)
        {
            builder.Entity<Hito>().HasKey(_ => _.Id);
            builder.Entity<Hito>().Property(_ => _.Description).HasMaxLength(100);

            builder.Entity<Hito>().Ignore(_ => _.DescriptionOld);
            builder.Entity<Hito>().Ignore(_ => _.UnitPriceOld);

            builder.Entity<Hito>().HasOne(x => x.Solfac).WithMany(x => x.Hitos).HasForeignKey(x => x.SolfacId);
        }
    }
}
