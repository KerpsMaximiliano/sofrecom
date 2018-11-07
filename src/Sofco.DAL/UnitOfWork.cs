using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.AdvancementAndRefund;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.Billing;
using Sofco.Core.DAL.Common;
using Sofco.Core.DAL.Report;
using Sofco.Core.DAL.Rrhh;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.DAL.Repositories.Admin;
using Sofco.DAL.Repositories.AdvancementAndRefund;
using Sofco.DAL.Repositories.AllocationManagement;
using Sofco.DAL.Repositories.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.DAL.Repositories.Reports;
using Sofco.DAL.Repositories.Rrhh;
using Sofco.DAL.Repositories.WorkTimeManagement;

namespace Sofco.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SofcoContext context;

        private IDbContextTransaction contextTransaction;

        private readonly IOptions<EmailConfig> emailConfig;

        private readonly IOptions<AppSetting> appSettingOptions;

        #region Admin

        private IUserRepository userRepository;
        private IRoleRepository roleRepository;
        private IGroupRepository groupRepository;
        private IModuleRepository moduleRepository;
        private IFunctionalityRepository functionalityRepository;
        private IUserGroupRepository userGroupRepository;
        private IMenuRepository menuRepository;
        private ISettingRepository settingRepository;
        private IRoleFunctionalityRepository roleFunctionalityRepository;
        private ICategoryRepository categoryRepository;
        private ITaskRepository taskRepository;
        private IAreaRepository areaRepository;
        private ISectorRepository sectorRepository;

        #endregion

        #region Billing

        private IInvoiceRepository invoiceRepository;
        private ISolfacRepository solfacRepository;
        private ISolfacReportRepository solfacReportRepository;
        private IPurchaseOrderRepository purchaseOrderRepository;
        private ICertificateRepository certificateRepository;
        private ISolfacCertificateRepository solfacCertificateRepository;
        private ICustomerRepository customerRepository;
        private IServiceRepository serviceRepository;
        private IProjectRepository projectRepository;

        #endregion

        #region AllocationManagement

        private IAllocationRepository allocationRepository;
        private IAnalyticRepository analyticRepository;
        private ICostCenterRepository costCenterRepository;
        private IEmployeeLicenseRepository employeeLicenseRepository;
        private IEmployeeRepository employeeRepository;
        private ILicenseTypeRepository licenseTypeRepository;
        private IEmployeeSyncActionRepository employeeSyncActionRepository;
        private IEmployeeHistoryRepository employeeHistoryRepository;
        private IHealthInsuranceRepository healthInsuranceRepository;
        private IPrepaidHealthRepository prepaidHealthRepository;

        #endregion

        #region HumanResource

        private ILicenseRepository licenseRepository;
        private ICloseDateRepository closeDateRepository;

        #endregion

        #region Common

        private IUtilsRepository utilsRepository;
        private IFileRepository fileRepository;
        private IUserDelegateRepository userDelegateRepository;

        #endregion

        #region WorkTimeManagement

        private IWorkTimeRepository workTimeRepository;
        private IUserApproverRepository userApproverRepository;
        private IHolidayRepository holidayRepository;

        #endregion

        #region WorkTimeManagement

        private IAdvancementRepository advancementRepository;

        #endregion

        public UnitOfWork(SofcoContext context, IOptions<EmailConfig> emailConfig, IOptions<AppSetting> appSettingOptions)
        {
            this.context = context;
            this.emailConfig = emailConfig;
            this.appSettingOptions = appSettingOptions;
        }

        #region Admin

        public IUserRepository UserRepository => userRepository ?? (userRepository = new UserRepository(context, emailConfig, appSettingOptions));
        public IRoleRepository RoleRepository => roleRepository ?? (roleRepository = new RoleRepository(context));
        public IGroupRepository GroupRepository => groupRepository ?? (groupRepository = new GroupRepository(context));
        public IModuleRepository ModuleRepository => moduleRepository ?? (moduleRepository = new ModuleRepository(context));
        public IFunctionalityRepository FunctionalityRepository => functionalityRepository ?? (functionalityRepository = new FunctionalityRepository(context));
        public IUserGroupRepository UserGroupRepository => userGroupRepository ?? (userGroupRepository = new UserGroupRepository(context));
        public IMenuRepository MenuRepository => menuRepository ?? (menuRepository = new MenuRepository(context));
        public ISettingRepository SettingRepository => settingRepository ?? (settingRepository = new SettingRepository(context));
        public IRoleFunctionalityRepository RoleFunctionalityRepository => roleFunctionalityRepository ?? (roleFunctionalityRepository = new RoleFunctionalityRepository(context));
        public ICategoryRepository CategoryRepository => categoryRepository ?? (categoryRepository = new CategoryRepository(context));
        public ITaskRepository TaskRepository => taskRepository ?? (taskRepository = new TaskRepository(context));
        public ISectorRepository SectorRepository => sectorRepository ?? (sectorRepository = new SectorRepository(context));

        #endregion

        #region Billing

        public IInvoiceRepository InvoiceRepository => invoiceRepository ?? (invoiceRepository = new InvoiceRepository(context));
        public ISolfacRepository SolfacRepository => solfacRepository ?? (solfacRepository = new SolfacRepository(context));
        public ISolfacReportRepository SolfacReportRepository => solfacReportRepository ?? (solfacReportRepository = new SolfacReportRepository(context));
        public IPurchaseOrderRepository PurchaseOrderRepository => purchaseOrderRepository ?? (purchaseOrderRepository = new PurchaseOrderRepository(context));
        public ICertificateRepository CertificateRepository => certificateRepository ?? (certificateRepository = new CertificateRepository(context));
        public ISolfacCertificateRepository SolfacCertificateRepository => solfacCertificateRepository ?? (solfacCertificateRepository = new SolfacCertificateRepository(context));
        public IAreaRepository AreaRepository => areaRepository ?? (areaRepository = new AreaRepository(context));
        public ICustomerRepository CustomerRepository => customerRepository ?? (customerRepository = new CustomerRepository(context));
        public IServiceRepository ServiceRepository => serviceRepository ?? (serviceRepository = new ServiceRepository(context));
        public IProjectRepository ProjectRepository => projectRepository ?? (projectRepository = new ProjectRepository(context));

        #endregion

        #region AllocationManagement

        public IAllocationRepository AllocationRepository => allocationRepository ?? (allocationRepository = new AllocationRepository(context));
        public IAnalyticRepository AnalyticRepository => analyticRepository ?? (analyticRepository = new AnalyticRepository(context));
        public ICostCenterRepository CostCenterRepository => costCenterRepository ?? (costCenterRepository = new CostCenterRepository(context));
        public IEmployeeLicenseRepository EmployeeLicenseRepository => employeeLicenseRepository ?? (employeeLicenseRepository = new EmployeeLicenseRepository(context));
        public IEmployeeRepository EmployeeRepository => employeeRepository ?? (employeeRepository = new EmployeeRepository(context));
        public ILicenseTypeRepository LicenseTypeRepository => licenseTypeRepository ?? (licenseTypeRepository = new LicenseTypeRepository(context));

        public IEmployeeSyncActionRepository EmployeeSyncActionRepository =>
            employeeSyncActionRepository ?? (employeeSyncActionRepository = new EmployeeSyncActionRepository(context));

        public IEmployeeHistoryRepository EmployeeHistoryRepository =>
            employeeHistoryRepository ?? (employeeHistoryRepository = new EmployeeHistoryRepository(context));

        public IHealthInsuranceRepository HealthInsuranceRepository =>
            healthInsuranceRepository ?? (healthInsuranceRepository = new HealthInsuranceRepository(context));

        public IPrepaidHealthRepository PrepaidHealthRepository =>
            prepaidHealthRepository ?? (prepaidHealthRepository = new PrepaidHealthRepository(context));

        #endregion

        #region HumanResource

        public ILicenseRepository LicenseRepository => licenseRepository ?? (licenseRepository = new LicenseRepository(context));
        public ICloseDateRepository CloseDateRepository => closeDateRepository ?? (closeDateRepository = new CloseDateRepository(context));

        #endregion

        #region Common

        public IUtilsRepository UtilsRepository => utilsRepository ?? (utilsRepository = new UtilsRepository(context));
        public IFileRepository FileRepository => fileRepository ?? (fileRepository = new FileRepository(context));
        public IUserDelegateRepository UserDelegateRepository => userDelegateRepository ?? (userDelegateRepository = new UserDelegateRepository(context));


        #endregion

        #region WorkTimeManagement

        public IWorkTimeRepository WorkTimeRepository => workTimeRepository ?? (workTimeRepository = new WorkTimeRepository(context));

        public IUserApproverRepository UserApproverRepository => userApproverRepository ?? (userApproverRepository = new UserApproverRepository(context));

        public IHolidayRepository HolidayRepository => holidayRepository ?? (holidayRepository = new HolidayRepository(context));

        #endregion

        #region WorkTimeManagement

        public IAdvancementRepository AdvancementRepository => advancementRepository ?? (advancementRepository = new AdvancementRepository(context));

        #endregion

        public void Save()
        {
            context.SaveChanges();
        }

        public void BeginTransaction()
        {
            contextTransaction = context.Database.BeginTransaction();
        }

        public void Rollback()
        {
            contextTransaction?.Rollback();
        }

        public void Commit()
        {
            contextTransaction?.Commit();
        }
    }
}
