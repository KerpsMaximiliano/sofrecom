using Microsoft.EntityFrameworkCore;
using Sofco.DAL.Mappings.Admin;
using Sofco.DAL.Mappings.AllocationManagement;
using Sofco.DAL.Mappings.Billing;
using Sofco.DAL.Mappings.Common;
using Sofco.DAL.Mappings.Reports;
using Sofco.DAL.Mappings.Rrhh;
using Sofco.DAL.Mappings.Utils;
using Sofco.DAL.Mappings.WorkTimeManagement;
using Sofco.Model.Models.Admin;
using Sofco.Model.Models.Billing;
using Sofco.Model.Relationships;
using Sofco.Model.Utils;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Models.Common;
using Sofco.Model.Models.Rrhh;
using Sofco.Model.Models.WorkTimeManagement;

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
        public DbSet<Functionality> Functionalities { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Task> Tasks { get; set; }

        // RelationShips
        public DbSet<RoleFunctionality> RoleFunctionality { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<SolfacCertificate> SolfacCertificates { get; set; }
        public DbSet<LicenseFile> LicenseFiles { get; set; }
        public DbSet<EmployeeCategory> EmployeeCategories { get; set; }
        public DbSet<PurchaseOrderAnalytic> PurchaseOrderAnalytics { get; set; }
        public DbSet<PurchaseOrderAmmountDetail> PurchaseOrderAmmountDetails { get; set; }

        // Billing Mappings
        public DbSet<Hito> Hitos { get; set; }
        public DbSet<HitoDetail> HitoDetails { get; set; }
        public DbSet<Solfac> Solfacs { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<SolfacHistory> SolfacHistories { get; set; }
        public DbSet<SolfacAttachment> SolfacAttachments { get; set; }
        public DbSet<InvoiceHistory> InvoiceHistories { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<Certificate> Certificates { get; set; }

        // Allocation Management Mappings
        public DbSet<Analytic> Analytics { get; set; }
        public DbSet<Allocation> Allocations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<LicenseType> LicenseTypes { get; set; }
        public DbSet<EmployeeLicense> EmployeeLicenses { get; set; }
        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<EmployeeSyncAction> EmployeeSyncActions { get; set; }
        public DbSet<EmployeeHistory> EmployeeHistory { get; set; }
        public DbSet<HealthInsurance> HealthInsurances { get; set; }
        public DbSet<PrepaidHealth> PrepaidHealths { get; set; }
        public DbSet<WorkTimeApproval> WorkTimeApprovals { get; set; }

        // Work Time Management
        public DbSet<WorkTime> WorkTimes { get; set; }

        public DbSet<Holiday> Holidays { get; set; }

        // Human Resources
        public DbSet<License> Licenses { get; set; }
        public DbSet<LicenseHistory> LicenseHistories { get; set; }

        // Common
        public DbSet<File> Files { get; set; }

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
        public DbSet<SoftwareLaw> SoftwareLaws { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<PurchaseOrderOptions> PurchaseOrderOptions { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<EmployeeEndReason> EmployeeEndReason { get; set; }

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
            builder.MapSetting();
            builder.MapCostCenter();
            builder.MapEmployeeSyncAction();
            builder.MapEmployeeHistory();
            builder.MapFile();
            builder.MapPurchaseOrder();
            builder.MapHealthInsurance();
            builder.MapPrepaidHealth();
            builder.MapCertificate();
            builder.MapUserDelegate();
            builder.MapSolfacCertificates();
            builder.MapLicense();
            builder.MapLicenseFiles();
            builder.MapWorkTimeApproval();
            builder.MapCategory();
            builder.MapTasks();
            builder.MapEmployeeCategory();
            builder.MapWorkTime();
            builder.MapPurchaseOrderAnalytic();
            builder.MapPurchaseOrderBalanceView();
            builder.MapPurchaseOrderBalanceDetailView();
        }
    }
}
