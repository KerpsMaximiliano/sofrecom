using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Relationships;

namespace Sofco.DAL.Mappings.Billing
{
    public static class SolfacCertificateMapping
    {
        public static void MapSolfacCertificates(this ModelBuilder builder)
        {
            builder.Entity<SolfacCertificate>().HasKey(t => new { t.SolfacId, t.CertificateId });

            builder.Entity<SolfacCertificate>()
                .HasOne(pt => pt.Solfac)
                .WithMany(p => p.SolfacCertificates)
                .HasForeignKey(pt => pt.SolfacId);

            builder.Entity<SolfacCertificate>()
                .HasOne(pt => pt.Certificate)
                .WithMany(t => t.SolfacCertificates)
                .HasForeignKey(pt => pt.CertificateId);
        }
    }
}
