using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Billing;

namespace Sofco.DAL.Mappings.Billing
{
    public static class ContactMapping
    {
        public static void MapContact(this ModelBuilder builder)
        {
            builder.Entity<Contact>().HasKey(_ => _.Id);
            builder.Entity<Contact>().Property(_ => _.CrmId).HasMaxLength(200);
            builder.Entity<Contact>().Property(_ => _.AccountId).HasMaxLength(200);
            builder.Entity<Contact>().Property(_ => _.Email).HasMaxLength(100);
            builder.Entity<Contact>().Property(_ => _.Name).HasMaxLength(200);
            builder.Entity<Contact>().Property(_ => _.Status).HasMaxLength(50);
        }
    }
}
