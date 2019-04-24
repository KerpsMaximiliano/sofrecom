using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Billing;

namespace Sofco.DAL.Mappings.Billing
{
    public static class OpportunityMapping
    {
        public static void MapOpportunity(this ModelBuilder builder)
        {
            builder.Entity<Opportunity>().HasKey(_ => _.Id);
            builder.Entity<Opportunity>().Property(_ => _.CrmId).HasMaxLength(200);
            builder.Entity<Opportunity>().Property(_ => _.ContactId).HasMaxLength(200);
            builder.Entity<Opportunity>().Property(_ => _.Name).HasMaxLength(200);
            builder.Entity<Opportunity>().Property(_ => _.Number).HasMaxLength(50);
            builder.Entity<Opportunity>().Property(_ => _.ParentContactName).HasMaxLength(200);
            builder.Entity<Opportunity>().Property(_ => _.ProjectManagerId).HasMaxLength(200);
            builder.Entity<Opportunity>().Property(_ => _.ProjectManagerName).HasMaxLength(200);
        }
    }
}
