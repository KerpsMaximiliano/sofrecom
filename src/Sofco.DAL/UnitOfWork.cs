﻿using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.AdvancementAndRefund;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.Billing;
using Sofco.Core.DAL.Common;
using Sofco.Core.DAL.ManagementReport;
using Sofco.Core.DAL.Provider;
using Sofco.Core.DAL.Recruitment;
using Sofco.Core.DAL.Report;
using Sofco.Core.DAL.RequestNote;
using Sofco.Core.DAL.Rrhh;
using Sofco.Core.DAL.Views;
using Sofco.Core.DAL.Workflow;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.DAL.Repositories.Admin;
using Sofco.DAL.Repositories.AdvancementAndRefund;
using Sofco.DAL.Repositories.AllocationManagement;
using Sofco.DAL.Repositories.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.DAL.Repositories.ManagementReport;
using Sofco.DAL.Repositories.Providers;
using Sofco.DAL.Repositories.Recruitment;
using Sofco.DAL.Repositories.Reports;
using Sofco.DAL.Repositories.RequestNote;
using Sofco.DAL.Repositories.Rrhh;
using Sofco.DAL.Repositories.Workflow;
using Sofco.DAL.Repositories.WorkTimeManagement;
using Sofco.Domain.Models.RequestNote;

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
        private IWorkflowStateRepository workflowStateRepository;

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
        private IContactRepository contactRepository;
        private IOpportunityRepository opportunityRepository;

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
        private IEmployeeEndNotificationRepository employeeEndNotificationRepository;
        private IHealthInsuranceRepository healthInsuranceRepository;
        private IPrepaidHealthRepository prepaidHealthRepository;
        private IEmployeeProfileHistoryRepository employeeProfileHistoryRepository;
        private IAnalyticsRenovationRepository analyticsRenovationRepository;

        #endregion

        #region HumanResource

        private ILicenseRepository licenseRepository;
        private ICloseDateRepository closeDateRepository;
        private IPrepaidImportedDataRepository prepaidImportedDataRepository;
        private IRrhhRepository rrhhRepository;

        #endregion

        #region Common

        private IUtilsRepository utilsRepository;
        private IFileRepository fileRepository;
        private ICurrencyExchangeRepository currencyExchangeRepository;
        private ILogRepository logRepository;
        private IDelegationRepository delegationRepository;

        #endregion

        #region Recruitment

        private IJobSearchRepository jobSearchRepository;
        private IApplicantRepository applicantRepository;
        private IJobSearchApplicantRepository jobSearchApplicantRepository;

        #endregion

        #region WorkTimeManagement

        private IWorkTimeRepository workTimeRepository;
        private IHolidayRepository holidayRepository;

        #endregion

        private IWorkflowRepository workflowRepository;

        private IUserSourceRepository userSourceRepository;


        #region Advancements

        private IAdvancementRepository advancementRepository;
        private IRefundRepository refundRepository;

        #endregion

        #region Management Report

        private ICostDetailRepository costDetailRepository;
        private IContractedDetailRepository contractedDetailRepository;
        private IManagementReportRepository managementReportRepository;
        private IManagementReportBillingRepository managementReportBillingRepository;
        private ICostDetailOtherRepository costDetailOtherRepository;
        private ICostDetailProfileRepository costDetailProfileRepository;
        private ICostDetailResourceRepository costDetailResourceRepository;
        private ICostDetailStaffRepository costDetailStaffRepository;


        #endregion
        private IProviderAreaRepository providerAreaRepository;

        private IProvidersRepository providersRepository;

        #region Request Note

        private IRequestNoteRepository requestNoteRepository;

        private IRequestNoteAnalitycRepository requestNoteAnalitycRepository;

        private IRequestNoteProviderRepository requestNoteProviderRepository;

        private IRequestNoteHistoryRepository requestNoteHistoryRepository;

        private IRequestNoteCommentRepository requestNoteCommentRepository;
        
        private IBaseRepository<RequestNoteFile> requestNoteFileRepository;

        private BaseRepository<RequestNoteProductService> requestNoteProductServiceRepository;

        private BaseRepository<RequestNoteTravel> requestNoteTravelRepository;

        private BaseRepository<RequestNoteTravelEmployee> requestNoteTravelEmployeeRepository;

        private BaseRepository<RequestNoteTraining> requestNoteTrainingRepository;

        private BaseRepository<RequestNoteTrainingEmployee> requestNoteTrainingEmployeeRepository;

        private BaseRepository<RequestNoteProviderSugg> requestNoteProviderSuggRepository;

        #endregion

        #region Buy Order
        private IBuyOrderRepository buyOrderRepository;
        private BaseRepository<BuyOrderHistory> buyOrderHistoryRepository;
        private BaseRepository<BuyOrderInvoice> buyOrderInvoiceRepository;
        private BaseRepository<BuyOrderProductService> buyOrderProductServiceRepository;

        private BaseRepository<BuyOrderInvoiceProductService> buyOrderInvoiceProductServiceRepository;
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
        public IGroupRepository GroupRepository => groupRepository ?? (groupRepository = new GroupRepository(context, emailConfig));
        public IModuleRepository ModuleRepository => moduleRepository ?? (moduleRepository = new ModuleRepository(context));
        public IFunctionalityRepository FunctionalityRepository => functionalityRepository ?? (functionalityRepository = new FunctionalityRepository(context));
        public IUserGroupRepository UserGroupRepository => userGroupRepository ?? (userGroupRepository = new UserGroupRepository(context));
        public IMenuRepository MenuRepository => menuRepository ?? (menuRepository = new MenuRepository(context));
        public ISettingRepository SettingRepository => settingRepository ?? (settingRepository = new SettingRepository(context));
        public IRoleFunctionalityRepository RoleFunctionalityRepository => roleFunctionalityRepository ?? (roleFunctionalityRepository = new RoleFunctionalityRepository(context));
        public ICategoryRepository CategoryRepository => categoryRepository ?? (categoryRepository = new CategoryRepository(context));
        public ITaskRepository TaskRepository => taskRepository ?? (taskRepository = new TaskRepository(context));
        public ISectorRepository SectorRepository => sectorRepository ?? (sectorRepository = new SectorRepository(context));
        public IWorkflowStateRepository WorkflowStateRepository => workflowStateRepository ?? (workflowStateRepository = new WorkflowStateRepository(context));


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
        public IContactRepository ContactRepository => contactRepository ?? (contactRepository = new ContactRepository(context));
        public IOpportunityRepository OpportunityRepository => opportunityRepository ?? (opportunityRepository = new OpportunityRepository(context));

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

        public IEmployeeEndNotificationRepository EmployeeEndNotificationRepository =>
            employeeEndNotificationRepository ?? (employeeEndNotificationRepository = new EmployeeEndNotificationRepository(context));

        public IHealthInsuranceRepository HealthInsuranceRepository =>
            healthInsuranceRepository ?? (healthInsuranceRepository = new HealthInsuranceRepository(context));

        public IPrepaidHealthRepository PrepaidHealthRepository => prepaidHealthRepository ?? (prepaidHealthRepository = new PrepaidHealthRepository(context));
        public IEmployeeProfileHistoryRepository EmployeeProfileHistoryRepository => employeeProfileHistoryRepository ?? (employeeProfileHistoryRepository = new EmployeeProfileHistoryRepository(context));

        public IAnalyticsRenovationRepository AnalyticsRenovationRepository => analyticsRenovationRepository ?? (analyticsRenovationRepository = new AnalyticsRenovationRepository(context));
        #endregion

        #region HumanResource

        public ILicenseRepository LicenseRepository => licenseRepository ?? (licenseRepository = new LicenseRepository(context));
        public ICloseDateRepository CloseDateRepository => closeDateRepository ?? (closeDateRepository = new CloseDateRepository(context));
        public IPrepaidImportedDataRepository PrepaidImportedDataRepository => prepaidImportedDataRepository ?? (prepaidImportedDataRepository = new PrepaidImportedDataRepository(context));
        public IRrhhRepository RrhhRepository => rrhhRepository ?? (rrhhRepository = new RrhhRepository(context));

        #endregion

        #region Common

        public IUtilsRepository UtilsRepository => utilsRepository ?? (utilsRepository = new UtilsRepository(context));
        public IFileRepository FileRepository => fileRepository ?? (fileRepository = new FileRepository(context));
        public ICurrencyExchangeRepository CurrencyExchangeRepository => currencyExchangeRepository ?? (currencyExchangeRepository = new CurrencyExchangeRepository(context));
        public ILogRepository LogRepository => logRepository ?? (logRepository = new LogRepository(context));
        public IDelegationRepository DelegationRepository => delegationRepository ?? (delegationRepository = new DelegationRepository(context));

        #endregion

        #region WorkTimeManagement

        public IWorkTimeRepository WorkTimeRepository => workTimeRepository ?? (workTimeRepository = new WorkTimeRepository(context));

        public IHolidayRepository HolidayRepository => holidayRepository ?? (holidayRepository = new HolidayRepository(context));

        #endregion

        public IWorkflowRepository WorkflowRepository => workflowRepository ?? (workflowRepository = new WorkflowRepository(context));

        public IUserSourceRepository UserSourceRepository => userSourceRepository ?? (userSourceRepository = new UserSourceRepository(context));

        #region Advancements

        public IAdvancementRepository AdvancementRepository => advancementRepository ?? (advancementRepository = new AdvancementRepository(context));
        public IRefundRepository RefundRepository => refundRepository ?? (refundRepository = new RefundRepository(context));

        #endregion

        #region Management Report

        public ICostDetailRepository CostDetailRepository => costDetailRepository ?? (costDetailRepository = new CostDetailRepository(context));
        public IContractedDetailRepository ContratedDetailRepository => contractedDetailRepository ?? (contractedDetailRepository = new ContractedDetailRepository(context));
        public IManagementReportRepository ManagementReportRepository => managementReportRepository ?? (managementReportRepository = new ManagementReportRepository(context));
        public IManagementReportBillingRepository ManagementReportBillingRepository => managementReportBillingRepository ?? (managementReportBillingRepository = new ManagementReportBillingRepository(context));
        public ICostDetailOtherRepository CostDetailOtherRepository => costDetailOtherRepository ?? (costDetailOtherRepository = new CostDetailOtherRepository(context));
        public ICostDetailProfileRepository CostDetailProfileRepository => costDetailProfileRepository ?? (costDetailProfileRepository = new CostDetailProfileRepository(context));
        public ICostDetailResourceRepository CostDetailResourceRepository => costDetailResourceRepository ?? (costDetailResourceRepository = new CostDetailResourceRepository(context));
        public ICostDetailStaffRepository CostDetailStaffRepository => costDetailStaffRepository ?? (costDetailStaffRepository = new CostDetailStaffRepository(context));

        #endregion

        #region Recruitment

        public IJobSearchRepository JobSearchRepository => jobSearchRepository ?? (jobSearchRepository = new JobSearchRepository(context));
        public IApplicantRepository ApplicantRepository => applicantRepository ?? (applicantRepository = new ApplicantRepository(context));
        public IJobSearchApplicantRepository JobSearchApplicantRepository => jobSearchApplicantRepository ?? (jobSearchApplicantRepository = new JobSearchApplicantRepository(context));

        #endregion

        #region Request Note
        public IProviderAreaRepository ProviderAreaRepository => providerAreaRepository ?? (providerAreaRepository = new ProviderAreaRepository(context));

        public IProvidersRepository ProvidersRepository => providersRepository ?? (providersRepository = new ProvidersRepository(context));

        public IRequestNoteRepository RequestNoteRepository => requestNoteRepository ?? (requestNoteRepository = new RequestNoteRepository(context));

        public IRequestNoteAnalitycRepository RequestNoteAnalitycRepository => requestNoteAnalitycRepository ?? (requestNoteAnalitycRepository = new RequestNoteAnalitycRepository(context));

        public IRequestNoteProviderRepository RequestNoteProviderRepository => requestNoteProviderRepository ?? (requestNoteProviderRepository = new RequestNoteProviderRepository(context));

        public IRequestNoteHistoryRepository RequestNoteHistoryRepository => requestNoteHistoryRepository ?? (requestNoteHistoryRepository = new RequestNoteHistoryRepository(context));
        public IRequestNoteCommentRepository RequestNoteCommentRepository => requestNoteCommentRepository ?? (requestNoteCommentRepository = new RequestNoteCommentRepository(context));

        public IBaseRepository<RequestNoteFile> RequestNoteFileRepository => requestNoteFileRepository ?? (requestNoteFileRepository = new RequestNoteFileRepository(context));

        public IBaseRepository<RequestNoteProductService> RequestNoteProductServiceRepository => requestNoteProductServiceRepository ?? (requestNoteProductServiceRepository = new BaseRepository<RequestNoteProductService>(context));

        public IBaseRepository<RequestNoteTravel> RequestNoteTravelRepository => requestNoteTravelRepository ?? (requestNoteTravelRepository = new BaseRepository<RequestNoteTravel>(context));

        public IBaseRepository<RequestNoteTravelEmployee> RequestNoteTravelEmployeeRepository => requestNoteTravelEmployeeRepository ?? (requestNoteTravelEmployeeRepository = new BaseRepository<RequestNoteTravelEmployee>(context));

        public IBaseRepository<RequestNoteTraining> RequestNoteTrainingRepository => requestNoteTrainingRepository ?? (requestNoteTrainingRepository = new BaseRepository<RequestNoteTraining>(context));

        public IBaseRepository<RequestNoteTrainingEmployee> RequestNoteTrainingEmployeeRepository => requestNoteTrainingEmployeeRepository ?? (requestNoteTrainingEmployeeRepository = new BaseRepository<RequestNoteTrainingEmployee>(context));

        public IBaseRepository<RequestNoteProviderSugg> RequestNoteProviderSuggRepository => requestNoteProviderSuggRepository ?? (requestNoteProviderSuggRepository = new BaseRepository<RequestNoteProviderSugg>(context));
        #endregion
        #region Buy Order (Req. Note)
        public IBuyOrderRepository BuyOrderRepository => buyOrderRepository ?? (buyOrderRepository = new BuyOrderRepository(context));
        public IBaseRepository<BuyOrderHistory> BuyOrderHistoryRepository => buyOrderHistoryRepository ?? (buyOrderHistoryRepository = new BaseRepository<BuyOrderHistory>(context));
        public IBaseRepository<BuyOrderInvoice> BuyOrderInvoiceRepository => buyOrderInvoiceRepository ?? (buyOrderInvoiceRepository = new BaseRepository<BuyOrderInvoice>(context));
        public IBaseRepository<BuyOrderProductService> BuyOrderProductServiceRepository => buyOrderProductServiceRepository ?? (buyOrderProductServiceRepository = new BaseRepository<BuyOrderProductService>(context));
        public IBaseRepository<BuyOrderInvoiceProductService> BuyOrderInvoiceProductServiceRepository => buyOrderInvoiceProductServiceRepository ?? (buyOrderInvoiceProductServiceRepository = new BaseRepository<BuyOrderInvoiceProductService>(context));

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
