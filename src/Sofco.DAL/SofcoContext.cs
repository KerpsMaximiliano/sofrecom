using Microsoft.EntityFrameworkCore;
using Sofco.DAL.Mappings.Admin;
using Sofco.DAL.Mappings.AllocationManagement;
using Sofco.DAL.Mappings.Billing;
using Sofco.DAL.Mappings.Utils;
using Sofco.Model.Models.Admin;
using Sofco.Model.Models.Billing;
using Sofco.Model.Relationships;
using Sofco.Model.Utils;
using Sofco.Model.Models.AllocationManagement;

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
        public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<Functionality> Functionalities { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<GlobalSetting> GlobalSettings { get; set; }

        // RelationShips
        public DbSet<RoleFunctionality> RoleFunctionality { get; set; }

        // Billing Mappings
        public DbSet<Hito> Hitos { get; set; }
        public DbSet<HitoDetail> HitoDetails { get; set; }
        public DbSet<Solfac> Solfacs { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<SolfacHistory> SolfacHistories { get; set; }
        public DbSet<SolfacAttachment> SolfacAttachments { get; set; }
        public DbSet<InvoiceHistory> InvoiceHistories { get; set; }

        // Allocation Management Mappings
        public DbSet<Analytic> Analytics { get; set; }
        public DbSet<Allocation> Allocations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<LicenseType> LicenseTypes { get; set; }
        public DbSet<EmployeeLicense> EmployeeLicenses { get; set; }
        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<EmployeeSyncAction> EmployeeSyncActions { get; set; }

        // Utils Mapping
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<ImputationNumber> ImputationNumbers { get; set; }
        public DbSet<Technology> Technologies { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<ClientGroup> ClientGroups { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PaymentTerm> PaymentTerms { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema(AppSchemaName);

            builder.MapRoles();
            builder.MapGroups();
            builder.MapModules();
            builder.MapUsers();
            builder.MapUserGroups();
            builder.MapFunctionalities();
            builder.MapSolfac();
            builder.MapUtils();
            builder.MapRoleFunctionality();
            builder.MapInvoice();
            builder.MapAnalytic();
            builder.MapAllocation();
            builder.MapEmployee();
            builder.MapLicenseType();
            builder.MapEmployeeLicense();
            builder.MapHitoDetails();
            builder.MapGlobalSetting();
            builder.MapCostCenter();
            builder.MapEmployeeSyncAction();
        }
    }
}
