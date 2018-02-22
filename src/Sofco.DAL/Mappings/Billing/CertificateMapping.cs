using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.Billing;

namespace Sofco.DAL.Mappings.Billing
{
    public static class CertificateMapping
    {
        public static void MapCertificate(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Certificate>().HasKey(_ => _.Id);
            builder.Entity<Certificate>().Property(_ => _.Name).HasMaxLength(150);
            builder.Entity<Certificate>().Property(_ => _.ClientExternalId).HasMaxLength(100);
            builder.Entity<Certificate>().Property(_ => _.ClientExternalName).HasMaxLength(100);
            builder.Entity<Certificate>().Property(_ => _.UpdateByUser).HasMaxLength(50);

            builder.Entity<Certificate>().HasOne(x => x.File).WithMany().HasForeignKey(x => x.FileId);
        }
    }
}
