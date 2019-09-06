using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.AdvancementAndRefund;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.Billing;
using Sofco.Core.DAL.Common;
using Sofco.Core.DAL.ManagementReport;
using Sofco.Core.DAL.Recruitment;
using Sofco.Core.DAL.Report;
using Sofco.Core.DAL.Rrhh;
using Sofco.Core.DAL.Workflow;
using Sofco.Core.DAL.WorkTimeManagement;

namespace Sofco.Core.DAL
{
    public interface IUnitOfWork
    {
        #region Admin
        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }
        IGroupRepository GroupRepository { get; }
        IModuleRepository ModuleRepository { get; }
        IFunctionalityRepository FunctionalityRepository { get; }
        IUserGroupRepository UserGroupRepository { get; }
        IMenuRepository MenuRepository { get; }
        ISettingRepository SettingRepository { get; }
        IRoleFunctionalityRepository RoleFunctionalityRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ITaskRepository TaskRepository { get; }
        ISectorRepository SectorRepository { get; }
        IAreaRepository AreaRepository { get; }
        IWorkflowStateRepository WorkflowStateRepository { get; }

        #endregion

        #region Billing

        IInvoiceRepository InvoiceRepository { get; }
        ISolfacRepository SolfacRepository { get; }
        ISolfacReportRepository SolfacReportRepository { get; }
        IPurchaseOrderRepository PurchaseOrderRepository { get; }
        ICertificateRepository CertificateRepository { get; }
        ISolfacCertificateRepository SolfacCertificateRepository { get; }
        ICustomerRepository CustomerRepository { get; }
        IServiceRepository ServiceRepository { get; }
        IProjectRepository ProjectRepository { get; }
        IContactRepository ContactRepository { get; }
        IOpportunityRepository OpportunityRepository { get; }

        #endregion

        #region AllocationManagement

        IAllocationRepository AllocationRepository { get; }
        IAnalyticRepository AnalyticRepository { get; }
        ICostCenterRepository CostCenterRepository { get; }
        IEmployeeLicenseRepository EmployeeLicenseRepository { get; }
        IEmployeeRepository EmployeeRepository { get; }
        ILicenseTypeRepository LicenseTypeRepository { get; }
        IEmployeeSyncActionRepository EmployeeSyncActionRepository { get; }
        IEmployeeHistoryRepository EmployeeHistoryRepository { get; }
        IEmployeeProfileHistoryRepository EmployeeProfileHistoryRepository { get; }
        IEmployeeEndNotificationRepository EmployeeEndNotificationRepository { get; }
        IHealthInsuranceRepository HealthInsuranceRepository { get; }
        IPrepaidHealthRepository PrepaidHealthRepository { get; }

        #endregion

        #region HumanResources

        ILicenseRepository LicenseRepository { get; }
        ICloseDateRepository CloseDateRepository { get; }
        IPrepaidImportedDataRepository PrepaidImportedDataRepository { get; }
        IRrhhRepository RrhhRepository { get; }

        #endregion

        #region Common

        IUtilsRepository UtilsRepository { get; }

        IFileRepository FileRepository { get; }

        IUserDelegateRepository UserDelegateRepository { get; }

        IUserApproverRepository UserApproverRepository { get; }
        ICurrencyExchangeRepository CurrencyExchangeRepository { get; }

        ILogRepository LogRepository { get; }

        IDelegationRepository DelegationRepository { get; }

        #endregion

        #region WorkTimeManagement

        IWorkTimeRepository WorkTimeRepository { get; }

        IHolidayRepository HolidayRepository { get; }

        #endregion

        IWorkflowRepository WorkflowRepository { get; }

        IUserSourceRepository UserSourceRepository { get; }

        #region Advancements

        IAdvancementRepository AdvancementRepository { get; }
        IRefundRepository RefundRepository { get; }

        #endregion

        #region Management Report

        ICostDetailRepository CostDetailRepository { get; }
        IContractedDetailRepository ContratedDetailRepository { get; }
        IManagementReportRepository ManagementReportRepository { get; }
        IManagementReportBillingRepository ManagementReportBillingRepository { get; }
        ICostDetailOtherRepository CostDetailOtherRepository { get; }
        ICostDetailProfileRepository CostDetailProfileRepository { get; }
        ICostDetailResourceRepository CostDetailResourceRepository { get; }
        ICostDetailStaffRepository CostDetailStaffRepository { get; }

        #endregion

        #region Recruitment

        IJobSearchRepository JobSearchRepository { get; }

        #endregion

        void BeginTransaction();
        void Rollback();
        void Commit();
        void Save();
    }
}
