using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.Billing;
using Sofco.Core.DAL.Common;
using Sofco.Core.DAL.Report;
using Sofco.Core.Services.Billing;

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
        IGlobalSettingRepository GlobalSettingRepository { get; }
        IRoleFunctionalityRepository RoleFunctionalityRepository { get; }

        #endregion

        #region Billing

        IInvoiceRepository InvoiceRepository { get; }
        ISolfacRepository SolfacRepository { get; }
        ISolfacReportRepository SolfacReportRepository { get; }
        IPurchaseOrderRepository PurchaseOrderRepository { get; }

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

        #endregion

        #region Common

        IUtilsRepository UtilsRepository { get; }
        IFileRepository FileRepository { get; }

        #endregion

        void Save();
    }
}
