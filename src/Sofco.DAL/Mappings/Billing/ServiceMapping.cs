using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Billing;

namespace Sofco.DAL.Mappings.Billing
{
    public static class ServiceMapping
    {
        public static void MapService(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Service>().HasKey(_ => _.Id);
            builder.Entity<Service>().Property(_ => _.CrmId).HasMaxLength(200);
            builder.Entity<Service>().Property(_ => _.Name).HasMaxLength(200);
            builder.Entity<Service>().Property(_ => _.AccountId).HasMaxLength(200);
            builder.Entity<Service>().Property(_ => _.AccountName).HasMaxLength(200);
            builder.Entity<Service>().Property(_ => _.Industry).HasMaxLength(200);
            builder.Entity<Service>().Property(_ => _.Manager).HasMaxLength(200);
            builder.Entity<Service>().Property(_ => _.ManagerId).HasMaxLength(200);
            builder.Entity<Service>().Property(_ => _.ServiceType).HasMaxLength(200);
            builder.Entity<Service>().Property(_ => _.SolutionType).HasMaxLength(200);
            builder.Entity<Service>().Property(_ => _.TechnologyType).HasMaxLength(200);
            builder.Entity<Service>().Property(_ => _.Analytic).HasMaxLength(200);
        }
    }
}
