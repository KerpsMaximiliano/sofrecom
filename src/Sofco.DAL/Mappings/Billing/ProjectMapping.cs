using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Billing;

namespace Sofco.DAL.Mappings.Billing
{
    public static class ProjectMapping
    {
        public static void MapProject(this ModelBuilder builder)
        {
            builder.Entity<Project>().HasKey(_ => _.Id);
            builder.Entity<Project>().Property(_ => _.CrmId).HasMaxLength(200);
            builder.Entity<Project>().Property(_ => _.Name).HasMaxLength(200);
            builder.Entity<Project>().Property(_ => _.AccountId).HasMaxLength(200);
            builder.Entity<Project>().Property(_ => _.OpportunityId).HasMaxLength(200);
            builder.Entity<Project>().Property(_ => _.OpportunityName).HasMaxLength(200);
            builder.Entity<Project>().Property(_ => _.OpportunityNumber).HasMaxLength(200);
            builder.Entity<Project>().Property(_ => _.Currency).HasMaxLength(200);
            builder.Entity<Project>().Property(_ => _.CurrencyId).HasMaxLength(200);
            builder.Entity<Project>().Property(_ => _.Integrator).HasMaxLength(200);
            builder.Entity<Project>().Property(_ => _.IntegratorId).HasMaxLength(200);
        }
    }
}
