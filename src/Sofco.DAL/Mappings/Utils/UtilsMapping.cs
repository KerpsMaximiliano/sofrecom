using Microsoft.EntityFrameworkCore;
using Sofco.Model.Utils;

namespace Sofco.DAL.Mappings.Utils
{
    public static class UtilsMapping
    {
        public static void MapUtils(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<DocumentType>().HasKey(_ => _.Id);
            builder.Entity<DocumentType>().Property(_ => _.Text).HasMaxLength(60);

            // Primary Key
            builder.Entity<Province>().HasKey(_ => _.Id);
            builder.Entity<Province>().Property(_ => _.Text).HasMaxLength(60);

            // Primary Key
            builder.Entity<ImputationNumber>().HasKey(_ => _.Id);
            builder.Entity<ImputationNumber>().Property(_ => _.Text).HasMaxLength(50);

            // Primary Key
            builder.Entity<Currency>().HasKey(_ => _.Id);
            builder.Entity<Currency>().Property(_ => _.Text).HasMaxLength(15);
            builder.Entity<Currency>().Property(_ => _.CrmDevelopmentId).HasMaxLength(100);
            builder.Entity<Currency>().Property(_ => _.CrmProductionId).HasMaxLength(100);

            // Primary Key
            builder.Entity<Solution>().HasKey(_ => _.Id);
            builder.Entity<Solution>().Property(_ => _.Text).HasMaxLength(60);

            // Primary Key
            builder.Entity<Technology>().HasKey(_ => _.Id);
            builder.Entity<Technology>().Property(_ => _.Text).HasMaxLength(60);

            // Primary Key
            builder.Entity<ClientGroup>().HasKey(_ => _.Id);
            builder.Entity<ClientGroup>().Property(_ => _.Text).HasMaxLength(60);

            // Primary Key
            builder.Entity<Product>().HasKey(_ => _.Id);
            builder.Entity<Product>().Property(_ => _.Text).HasMaxLength(60);

            // Primary Key
            builder.Entity<PaymentTerm>().HasKey(_ => _.Id);
            builder.Entity<PaymentTerm>().Property(_ => _.Text).HasMaxLength(60);

            // Primary Key
            builder.Entity<SoftwareLaw>().HasKey(_ => _.Id);
            builder.Entity<SoftwareLaw>().Property(_ => _.Text).HasMaxLength(60);

            // Primary Key
            builder.Entity<ServiceType>().HasKey(_ => _.Id);
            builder.Entity<ServiceType>().Property(_ => _.Text).HasMaxLength(60);

            // Primary Key
            builder.Entity<PurchaseOrderOptions>().HasKey(_ => _.Id);
            builder.Entity<PurchaseOrderOptions>().Property(_ => _.Text).HasMaxLength(60);

            // Primary Key
            builder.Entity<Sector>().HasKey(_ => _.Id);
            builder.Entity<Sector>().Property(_ => _.Text).HasMaxLength(100);

            // Primary Key
            builder.Entity<Area>().HasKey(_ => _.Id);
            builder.Entity<Area>().Property(_ => _.Text).HasMaxLength(100);
            builder.Entity<Area>().HasOne(x => x.ResponsableUser).WithMany(x => x.Areas).HasForeignKey(x => x.ResponsableUserId);
        }
    }
}
