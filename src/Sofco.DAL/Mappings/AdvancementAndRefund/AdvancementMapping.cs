using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.DAL.Mappings.AdvancementAndRefund
{
    public static class AdvancementMapping
    {
        public static void MapAdvancement(this ModelBuilder builder)
        {
            builder.Entity<Advancement>().HasKey(t => t.Id);
            builder.Entity<Advancement>().HasOne(x => x.UserApplicant).WithMany(x => x.Advancements).HasForeignKey(x => x.UserApplicantId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Advancement>().HasOne(x => x.Analytic).WithMany(x => x.Advancements).HasForeignKey(x => x.AnalyticId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Advancement>().HasMany(x => x.Details).WithOne(x => x.Advancement).HasForeignKey(x => x.AdvancementId);
            builder.Entity<Advancement>().HasOne(x => x.Currency).WithMany(x => x.Advancements).HasForeignKey(x => x.CurrencyId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Advancement>().HasOne(x => x.AdvancementReturnForm).WithMany(x => x.Advancements).HasForeignKey(x => x.AdvancementReturnFormId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AdvancementDetail>().HasKey(t => t.Id);
            builder.Entity<AdvancementDetail>().Property(x => x.Description).HasMaxLength(400);
            builder.Entity<AdvancementDetail>().HasOne(x => x.Advancement).WithMany(x => x.Details).HasForeignKey(x => x.AdvancementId);
        }
    }
}
