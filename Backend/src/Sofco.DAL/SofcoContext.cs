using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models;
using Sofco.DAL.Mapping;
using Sofco.Model.Relationships;

namespace Sofco.DAL
{
    public class SofcoContext : DbContext
    {
        public SofcoContext(DbContextOptions<SofcoContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.MapCustomers();
            builder.MapRoles();
            builder.MapGroups();
            builder.MapMenus();
            builder.MapUsers();
            builder.MapUserGroupsUser();
        }
    }
}
