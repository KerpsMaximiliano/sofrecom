using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.DAL.Mappings.AdvancementAndRefund
{
    public static class AdvancementRefundMapping
    {
        public static void MapAdvancementRefund(this ModelBuilder builder)
        {
            builder.Entity<AdvancementRefund>().HasKey(t => new { t.AdvancementId, t.RefundId });

            builder.Entity<AdvancementRefund>()
                .HasOne(pt => pt.Advancement)
                .WithMany(p => p.AdvancementRefunds)
                .HasForeignKey(pt => pt.AdvancementId);

            builder.Entity<AdvancementRefund>()
                .HasOne(pt => pt.Refund)
                .WithMany(t => t.AdvancementRefunds)
                .HasForeignKey(pt => pt.RefundId);
        }
    }
}
