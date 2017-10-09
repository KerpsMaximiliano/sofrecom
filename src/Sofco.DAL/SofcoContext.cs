﻿using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models;
using Sofco.DAL.Mappings.Admin;
using Sofco.DAL.Mappings.Billing;
using Sofco.DAL.Mappings.Utils;
using Sofco.Model.Models.Admin;
using Sofco.Model.Models.Billing;
using Sofco.Model.Relationships;
using Sofco.Model.Utils;

namespace Sofco.DAL
{
    public class SofcoContext : DbContext
    {
        public const string AppSchemaName = "app";

        public SofcoContext(DbContextOptions<SofcoContext> options)
            : base(options)
        {
        }

        // Admin Mappings
        public DbSet<Group> Groups { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<Functionality> Functionalities { get; set; }
        public DbSet<Module> Modules { get; set; }

        // RelationShips
        public DbSet<RoleFunctionality> RoleFunctionality { get; set; }

        // Billing Mappings
        public DbSet<Hito> Hitos { get; set; }
        public DbSet<Solfac> Solfacs { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<SolfacHistory> SolfacHistories { get; set; }
        public DbSet<SolfacAttachment> SolfacAttachments { get; set; }
        public DbSet<InvoiceHistory> InvoiceHistories { get; set; }

        // Utils Mapping
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<ImputationNumber> ImputationNumbers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema(AppSchemaName);

            builder.MapRoles();
            builder.MapGroups();
            builder.MapModules();
            builder.MapMenus();
            builder.MapUsers();
            builder.MapUserGroups();
            builder.MapFunctionalities();
            builder.MapSolfac();
            builder.MapUtils();
            builder.MapRoleFunctionality();
            builder.MapInvoice();
        }
    }
}
