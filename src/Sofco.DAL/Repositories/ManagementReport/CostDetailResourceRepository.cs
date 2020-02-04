using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.ManagementReport;

namespace Sofco.DAL.Repositories.ManagementReport
{
    public class CostDetailResourceRepository : BaseRepository<CostDetailResource>, ICostDetailResourceRepository
    {
        public CostDetailResourceRepository(SofcoContext context) : base(context)
        {
        }

        public IList<CostDetailResource> GetByAnalyticAndDates(int managementReportId, DateTime firstMonthDate, DateTime lastMonthDate)
        {
            return context.CostDetailResources
                .Include(x => x.CostDetail)
                .Include(x => x.BudgetType)
                .Where(x => x.CostDetail.ManagementReportId == managementReportId && x.CostDetail.MonthYear.Date >= firstMonthDate && x.CostDetail.MonthYear.Date <= lastMonthDate)
                .ToList();
        }

        public IList<CostDetailResource> GetByDate(DateTime date)
        {
            return context.CostDetailResources
                .Include(x => x.CostDetail)
                .Include(x => x.BudgetType)
                .ThenInclude(x => x.ManagementReport)
                .Where(x => x.CostDetail.MonthYear.Date == date.Date).ToList();
        }

        public int GetIdIfExist(int costDetailId, int resourceEmployeeId, int auxBudgetTypeId)
        {
            var entity = context.CostDetailResources.SingleOrDefault(x => x.CostDetailId == costDetailId && x.EmployeeId == resourceEmployeeId && x.BudgetTypeId == auxBudgetTypeId);

            if (entity != null)
            {
                return entity.Id;
            }

            return 0;
        }
    }
}
