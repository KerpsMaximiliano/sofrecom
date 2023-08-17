using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Billing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class AnalyticsRenovationMapping
    {
        public static void MapAnalyticsRenovation(this ModelBuilder builder)
        {
            builder.Entity<AnalyticsRenovation>().HasKey(x => x.Id);
            builder.Entity<AnalyticsRenovation>().Property(x => x.Renovation);
            builder.Entity<AnalyticsRenovation>().Property(x => x.AnalyticId);
            builder.Entity<AnalyticsRenovation>().Property(x => x.Orden);
            builder.Entity<AnalyticsRenovation>().Property(x => x.OpportunityId);
            builder.Entity<AnalyticsRenovation>().Property(x => x.ModifiedBy);
            builder.Entity<AnalyticsRenovation>().Property(x => x.CreatedBy);

            builder.Entity<AnalyticsRenovation>().HasOne(x => x.Analytic).WithMany(x => x.AnalyticsRenovations).HasForeignKey(x => x.AnalyticId);
            builder.Entity<AnalyticsRenovation>().HasOne(x => x.Opportunity).WithMany(x => x.AnalyticsRenovations).HasForeignKey(x => x.OpportunityId);
        }
    }
}
