using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class AnalyticMapping
    {
        public static void MapAnalytic(this ModelBuilder builder)
        {
            builder.Entity<Analytic>().HasKey(_ => _.Id);
            builder.Entity<Analytic>().Property(_ => _.AmountEarned).HasMaxLength(500);
            builder.Entity<Analytic>().Property(_ => _.AmountProject).HasMaxLength(500);
            builder.Entity<Analytic>().Property(_ => _.ClientExternalId).HasMaxLength(150);
            builder.Entity<Analytic>().Property(_ => _.ClientExternalName).HasMaxLength(150);
            builder.Entity<Analytic>().Property(_ => _.ClientProjectTfs).HasMaxLength(150);
            builder.Entity<Analytic>().Property(_ => _.CommercialManager).HasMaxLength(150);
            builder.Entity<Analytic>().Property(_ => _.ContractNumber).HasMaxLength(50);
            builder.Entity<Analytic>().Property(_ => _.Description).HasMaxLength(500);
            builder.Entity<Analytic>().Property(_ => _.Name).HasMaxLength(200);
            builder.Entity<Analytic>().Property(_ => _.Proposal).HasMaxLength(200);
            builder.Entity<Analytic>().Property(_ => _.Service).HasMaxLength(50);
            builder.Entity<Analytic>().Property(_ => _.Title).HasMaxLength(150);
            builder.Entity<Analytic>().Property(_ => _.UsersQv).HasMaxLength(500);

            //builder.Entity<Analytic>().HasOne(x => x.Director).WithMany(x => x.Analytics).HasForeignKey(x => x.DirectorId).OnDelete(DeleteBehavior.SetNull);
            //builder.Entity<Analytic>().HasOne(x => x.Manager).WithMany(x => x.Analytics).HasForeignKey(x => x.ManagerId).OnDelete(DeleteBehavior.SetNull); ;

            builder.Entity<Analytic>().HasOne(x => x.Technology).WithMany(x => x.Analytics).HasForeignKey(x => x.TechnologyId);
            builder.Entity<Analytic>().HasOne(x => x.Solution).WithMany(x => x.Analytics).HasForeignKey(x => x.SolutionId);
            builder.Entity<Analytic>().HasOne(x => x.Currency).WithMany(x => x.Analytics).HasForeignKey(x => x.CurrencyId);
            builder.Entity<Analytic>().HasOne(x => x.Activity).WithMany(x => x.Analytics).HasForeignKey(x => x.ActivityId);
            builder.Entity<Analytic>().HasOne(x => x.ClientGroup).WithMany(x => x.Analytics).HasForeignKey(x => x.ClientGroupId);
            builder.Entity<Analytic>().HasOne(x => x.Product).WithMany(x => x.Analytics).HasForeignKey(x => x.ProductId);
        }
    }
}
