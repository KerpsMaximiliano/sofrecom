using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class AnalyticMapping
    {
        public static void MapAnalytic(this ModelBuilder builder)
        {
            builder.Entity<Analytic>().HasKey(_ => _.Id);
            builder.Entity<Analytic>().Property(_ => _.ClientExternalId).HasMaxLength(150);
            builder.Entity<Analytic>().Property(_ => _.ClientExternalName).HasMaxLength(150);
            builder.Entity<Analytic>().Property(_ => _.Name).HasMaxLength(200);
            builder.Entity<Analytic>().Property(_ => _.Proposal).HasMaxLength(200);
            builder.Entity<Analytic>().Property(_ => _.Service).HasMaxLength(200);
            builder.Entity<Analytic>().Property(_ => _.Title).HasMaxLength(150);
            builder.Entity<Analytic>().Property(_ => _.UsersQv).HasMaxLength(500);

            builder.Entity<Analytic>().HasOne(x => x.Sector).WithMany(x => x.Analytics).HasForeignKey(x => x.SectorId);
            builder.Entity<Analytic>().HasOne(x => x.Manager).WithMany(x => x.Analytics2).HasForeignKey(x => x.ManagerId);
            builder.Entity<Analytic>().HasOne(x => x.CommercialManager).WithMany(x => x.Analytics3).HasForeignKey(x => x.CommercialManagerId);

            builder.Entity<Analytic>().HasOne(x => x.Technology).WithMany(x => x.Analytics).HasForeignKey(x => x.TechnologyId);
            builder.Entity<Analytic>().HasOne(x => x.Solution).WithMany(x => x.Analytics).HasForeignKey(x => x.SolutionId);
            builder.Entity<Analytic>().HasOne(x => x.Activity).WithMany(x => x.Analytics).HasForeignKey(x => x.ActivityId);
            builder.Entity<Analytic>().HasOne(x => x.ClientGroup).WithMany(x => x.Analytics).HasForeignKey(x => x.ClientGroupId);
            builder.Entity<Analytic>().HasOne(x => x.CostCenter).WithMany(x => x.Analytics).HasForeignKey(x => x.CostCenterId);
            builder.Entity<Analytic>().HasOne(x => x.SoftwareLaw).WithMany(x => x.Analytics).HasForeignKey(x => x.SoftwareLawId);
            builder.Entity<Analytic>().HasOne(x => x.ServiceType).WithMany(x => x.Analytics).HasForeignKey(x => x.ServiceTypeId);
        }
    }
}
