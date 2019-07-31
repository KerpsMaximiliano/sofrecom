using Sofco.Core.DAL.Common;
using Sofco.Core.Models.ManagementReport;
using System;
using Sofco.Domain.Models.ManagementReport;
using System.Collections.Generic;

namespace Sofco.Core.DAL.ManagementReport
{
    public interface IManagementReportRepository : IBaseRepository<Domain.Models.ManagementReport.ManagementReport>
    {
        Domain.Models.ManagementReport.ManagementReport GetById(int IdManamentReport);
        Domain.Models.ManagementReport.ManagementReport GetWithAnalytic(int managementReportId);
        void UpdateStatus(Domain.Models.ManagementReport.ManagementReport report);
        bool AllMonthsAreClosed(int managementReportId);
        List<Domain.Models.ManagementReport.ManagementReport> GetByDate(DateTime date);
        Domain.Models.ManagementReport.ManagementReport GetStaffById(int id);
        void AddBudget(Budget budget);
        Budget GetBudget(int id);
        List<Budget> GetBudgetByIdStaff(int managementReportId);
        void UpdateBudget(Budget budget);
        void DeleteBudget(Budget budget);
        List<BudgetType> GetTypesBudget();
        List<CostDetailCategories> GetCategories();
    }
}
