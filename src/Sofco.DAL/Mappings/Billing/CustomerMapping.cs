using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Billing;

namespace Sofco.DAL.Mappings.Billing
{
    public static class CustomerMapping
    {
        public static void MapCustomer(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Customer>().HasKey(_ => _.Id);
            builder.Entity<Customer>().Property(_ => _.CrmId).HasMaxLength(200);
            builder.Entity<Customer>().Property(_ => _.Address).HasMaxLength(200);
            builder.Entity<Customer>().Property(_ => _.Cuit).HasMaxLength(200);
            builder.Entity<Customer>().Property(_ => _.City).HasMaxLength(200);
            builder.Entity<Customer>().Property(_ => _.Contact).HasMaxLength(200);
            builder.Entity<Customer>().Property(_ => _.Country).HasMaxLength(200);
            builder.Entity<Customer>().Property(_ => _.CurrencyDescription).HasMaxLength(200);
            builder.Entity<Customer>().Property(_ => _.CurrencyId).HasMaxLength(200);
            builder.Entity<Customer>().Property(_ => _.Name).HasMaxLength(200);
            builder.Entity<Customer>().Property(_ => _.PaymentTermCode).HasMaxLength(200);
            builder.Entity<Customer>().Property(_ => _.PaymentTermDescription).HasMaxLength(200);
            builder.Entity<Customer>().Property(_ => _.PostalCode).HasMaxLength(200);
            builder.Entity<Customer>().Property(_ => _.Province).HasMaxLength(200);
            builder.Entity<Customer>().Property(_ => _.Telephone).HasMaxLength(200);
            builder.Entity<Customer>().Property(_ => _.OwnerId).HasMaxLength(200);
        }
    }
}
