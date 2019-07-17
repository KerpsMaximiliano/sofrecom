using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.DAL.Mappings.AdvancementAndRefund
{
    public static class AdvancementMapping
    {
        public static void MapAdvancement(this ModelBuilder builder)
        {
            builder.Entity<Advancement>().HasKey(t => t.Id);
            builder.Entity<Advancement>().Property(x => x.Description).HasMaxLength(1000);
            builder.Entity<Advancement>().Property(x => x.AdvancementReturnForm).HasMaxLength(200);
            builder.Entity<Advancement>().HasOne(x => x.UserApplicant).WithMany(x => x.Advancements).HasForeignKey(x => x.UserApplicantId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Advancement>().HasOne(x => x.Currency).WithMany(x => x.Advancements).HasForeignKey(x => x.CurrencyId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Advancement>().HasOne(x => x.MonthsReturn).WithMany(x => x.Advancements).HasForeignKey(x => x.MonthsReturnId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Advancement>().HasOne(x => x.Status).WithMany(x => x.Advancements).HasForeignKey(x => x.StatusId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Advancement>().HasOne(x => x.Workflow).WithMany(x => x.Advancements).HasForeignKey(x => x.WorkflowId);
            builder.Entity<Advancement>().HasMany(x => x.Histories).WithOne(x => x.Advancement).HasForeignKey(x => x.AdvancementId);

            builder.Entity<AdvancementHistory>().HasKey(x => x.Id);
            builder.Entity<AdvancementHistory>().Property(x => x.Comment).HasMaxLength(400);
            builder.Entity<AdvancementHistory>().Property(x => x.UserName).HasMaxLength(100);

            builder.Entity<AdvancementHistory>().HasOne(x => x.StatusFrom).WithMany(x => x.AdvancementHistories).HasForeignKey(x => x.StatusFromId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<AdvancementHistory>().HasOne(x => x.StatusTo).WithMany(x => x.AdvancementHistories2).HasForeignKey(x => x.StatusToId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AdvancementHistory>().HasOne(x => x.Advancement).WithMany(x => x.Histories).HasForeignKey(x => x.AdvancementId);

            builder.Entity<SalaryDiscount>().HasKey(t => t.Id);
            builder.Entity<SalaryDiscount>().HasOne(x => x.Employee).WithMany(x => x.SalaryDiscounts).HasForeignKey(x => x.EmployeeId);
        }
    }
}
