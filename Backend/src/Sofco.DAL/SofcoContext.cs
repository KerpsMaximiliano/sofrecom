using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models;
using Sofco.DAL.Mapping;

namespace Sofco.DAL
{
    public class SofcoContext : DbContext
    {
        public SofcoContext(DbContextOptions<SofcoContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.MapCustomers();
            builder.MapRoles();
            builder.MapUserGroups();
        }
    }
}
