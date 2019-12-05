using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.ManagementReport;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Sofco.DAL.Repositories.ManagementReport
{
    public class CostDetailRepository : BaseRepository<CostDetail>, ICostDetailRepository
    {
        public CostDetailRepository(SofcoContext context) : base(context)
        {
        }

        public IList<CostDetail> GetByManagementReport(int managementReportId)
        {
            return context.CostDetails
                    .Where(x => x.ManagementReportId == managementReportId)
                    .Include(x => x.CostDetailOthers)
                    .ToList();
        }

        //public List<CostDetailType> GetResourceTypes()
        //{
        //    return context.CostDetailTypes.OrderBy(t => t.Id).ToList();
        //}

        //public List<CostDetailSubtype> GetSubtypes(int idType)
        //{
        //    return context.CostDetailSubtype
        //        .Where(s => s.CostDetailTypeId == idType)
        //        .OrderBy(s => s.Name)
        //        .ToList();
        //}

        public CostDetail GetByManagementReportAndMonthYear(int managementReportId, DateTime monthYear)
        {
            return context.CostDetails
                .Include(x => x.CostDetailProfiles)
                .Include(x => x.CostDetailOthers)
                    .ThenInclude(b => b.CostDetailSubcategory)
                        .ThenInclude(z => z.CostDetailCategory)
                .Include(x => x.ContratedDetails)
                .Include(x => x.CostDetailResources)
                    .ThenInclude(y => y.BudgetType)
                .Include(x => x.CostDetailStaff)                            
                        .ThenInclude(y => y.CostDetailSubcategory)
                            .ThenInclude(z => z.CostDetailCategory)
                .Include(x => x.CostDetailStaff)
                    .ThenInclude(y => y.BudgetType)
                .FirstOrDefault(x => x.ManagementReportId == managementReportId
                                     && new DateTime(x.MonthYear.Year, x.MonthYear.Month, 1).Date == new DateTime(monthYear.Year, monthYear.Month, 1).Date);
        }

        public void UpdateTotals(CostDetail costDetailMonth)
        {
            context.Entry(costDetailMonth).Property("Provision").IsModified = true;
            context.Entry(costDetailMonth).Property("TotalBilling").IsModified = true;
            context.Entry(costDetailMonth).Property("TotalProvisioned").IsModified = true;
        }

        public void UpdateHasReal(CostDetail costDetailMonth)
        {
            context.Entry(costDetailMonth).Property("HasReal").IsModified = true;
        }

        public CostDetail GetWithResourceDetails(int managementReportId, DateTime date)
        {
            return context.CostDetails
                    .Include(x => x.CostDetailResources)
                        .ThenInclude(y => y.BudgetType)
                    .SingleOrDefault(x =>
                        x.MonthYear.Date == date.Date && x.ManagementReportId == managementReportId);
        }

        public void Close(CostDetail detailCost)
        {
            context.Entry(detailCost).Property("Closed").IsModified = true;
        }

        public IList<CostDetail> GetByManagementReportAndDates(int managementReportId, DateTime startDate, DateTime endDate)
        {
            return context.CostDetails
               .Include(x => x.CostDetailResources)
                    .ThenInclude(y => y.BudgetType)
               .Include(x => x.CostDetailProfiles)
               .Include(x => x.CostDetailOthers)
               .Include(x => x.ContratedDetails)
               .Where(x => x.ManagementReportId == managementReportId && x.MonthYear.Date >= startDate.Date &&
                            x.MonthYear.Date <= endDate.Date).ToList();
        }

        public List<CostDetailCategories> GetCategories()
        {
            return context.CostDetailCategories
                .Include(x=> x.Subcategories)
                .OrderBy(t => t.Id).ToList();
        }

        public List<CostDetailSubcategories> GetSubcategories()
        {
            return context.CostDetailSubcategories.OrderBy(t => t.Id).ToList();
        }

        public void UpdateTotalProvisioned(CostDetail costDetail)
        {
            context.Entry(costDetail).Property("TotalProvisioned").IsModified = true;
        }
    }
}
