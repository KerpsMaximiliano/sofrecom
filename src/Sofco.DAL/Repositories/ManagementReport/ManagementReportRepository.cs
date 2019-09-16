using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using System.Linq;
using Sofco.Core.Models.ManagementReport;
using System;
using Sofco.Domain.Models.ManagementReport;
using System.Collections.Generic;

namespace Sofco.DAL.Repositories.ManagementReport
{
    public class ManagementReportRepository : BaseRepository<Domain.Models.ManagementReport.ManagementReport>, IManagementReportRepository
    {
        public ManagementReportRepository(SofcoContext context) : base(context)
        {
        }

        public Domain.Models.ManagementReport.ManagementReport GetById(int IdManamentReport)
        {
            var data = context.ManagementReports
                .Include(mr => mr.Analytic)
                .Include(mr => mr.CostDetails)
                    .ThenInclude(x => x.CostDetailResources)
                .Include(mr => mr.CostDetails)
                    .ThenInclude(x => x.CostDetailProfiles)
                .Include(mr => mr.CostDetails)
                    .ThenInclude(x => x.CostDetailOthers)
                .Include(mr => mr.CostDetails)
                    .ThenInclude(x => x.ContratedDetails)
                .Include(mr => mr.CostDetails)
                    .ThenInclude(x => x.CostDetailStaff)
                        .ThenInclude(y => y.CostDetailSubcategory)
                .Include(mr => mr.CostDetails)
                    .ThenInclude(x => x.CostDetailStaff)
                        .ThenInclude(y => y.BudgetType)
                .Include(mr=> mr.State)
                .SingleOrDefault(mr => mr.Id == IdManamentReport);

            return data;
        }

        public Domain.Models.ManagementReport.ManagementReport GetWithAnalytic(int managementReportId)
        {
            var data = context.ManagementReports
                .Include(x => x.Analytic)
                .ThenInclude(x => x.Manager)
                .SingleOrDefault(mr => mr.Id == managementReportId);

            return data;
        }

        public void UpdateStatus(Domain.Models.ManagementReport.ManagementReport report)
        {
            context.Entry(report).Property("Status").IsModified = true;
        }

        public bool AllMonthsAreClosed(int managementReportId)
        {
            return context.CostDetails.Where(x => x.ManagementReportId == managementReportId).All(x => x.Closed);
        }

        public Domain.Models.ManagementReport.ManagementReport GetStaffById(int id)
        {
            return context.ManagementReports
                .Include(x => x.Analytic)
                .ThenInclude(x => x.Sector)
                .Include(x => x.Analytic)
                .ThenInclude(x => x.Manager)
                .Include(x => x.Budgets)
                .SingleOrDefault(x => x.Id == id);
        }

        public List<Domain.Models.ManagementReport.ManagementReport> GetByDate(DateTime date)
        {
            var data = context.ManagementReports
                .Include(x => x.Analytic)
                .ThenInclude(x => x.Manager)
                .Where(x => x.StartDate.Date <= date.Date && x.EndDate.Date >= date.Date)
                .ToList();

            return data;
        }
        
        public void AddBudget(Budget budget)
        {
            context.Budgets.Add(budget);
        }

        public Budget GetBudget(int id)
        {
            return context.Budgets.SingleOrDefault(x => x.Id == id);
        }

        public List<Budget> GetBudgetByIdStaff(int managementReportId)
        {
            return context.Budgets
                    .Where(x => x.ManagementReportId == managementReportId)
                    .ToList();
        }

        public void UpdateBudget(Budget budget)
        {
            context.Budgets.Update(budget);
        }

        public void DeleteBudget(Budget budget)
        {
            context.Budgets.Remove(budget);
        }

        public List<BudgetType> GetTypesBudget()
        {
            return context.BudgetType.OrderBy(x => x.Id).ToList();
        }

        public List<CostDetailCategories> GetCategories()
        {
            return context.CostDetailCategories
                    .Include(x => x.Subcategories)
                    .OrderBy(x => x.Name)
                    .ToList();
        }

        public bool Exist(int id)
        {
            return context.ManagementReports.Any(x => x.Id == id);
        }

        public void AddComment(ManagementReportComment mrComment)
        {
            context.ManagementReportComments.Add(mrComment);
        }

        public IList<ManagementReportComment> GetComments(int id)
        {
            return context.ManagementReportComments.Where(x => x.ManagementReportId == id).ToList();
        }

        public Domain.Models.ManagementReport.ManagementReport GetWithCostDetailsAndBillings(int id)
        {
            return context.ManagementReports
                .Include(x => x.CostDetails)
                .Include(x => x.Analytic)
                .Include(x => x.Billings)
                .SingleOrDefault(x => x.Id == id);
        }

        public IList<ManagementReportBilling> GetBillingsByMonthYear(DateTime monthYear, int managementReportId)
        {
            return context.ManagementReportBillings
                .Where(x => x.ManagementReportId == managementReportId && x.MonthYear.Date >= monthYear.Date)
                .ToList();
        }
    }
}
