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
            builder.Entity<Advancement>().Property(x => x.Description).HasMaxLength(400);
            builder.Entity<Advancement>().HasOne(x => x.UserApplicant).WithMany(x => x.Advancements).HasForeignKey(x => x.UserApplicantId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Advancement>().HasOne(x => x.Authorizer).WithMany(x => x.Advancements2).HasForeignKey(x => x.AuthorizerId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Advancement>().HasOne(x => x.Currency).WithMany(x => x.Advancements).HasForeignKey(x => x.CurrencyId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Advancement>().HasOne(x => x.AdvancementReturnForm).WithMany(x => x.Advancements).HasForeignKey(x => x.AdvancementReturnFormId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Advancement>().HasOne(x => x.Status).WithMany(x => x.Advancements).HasForeignKey(x => x.StatusId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
