using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models;

namespace Sofco.DAL.Mapping
{
    public static class CustomerMapping
    {
        public static void MapCustomers(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Customer>().HasKey(_ => _.Id);
            builder.Entity<Customer>().Property(_ => _.Description).HasMaxLength(50);
        }
    }
}
