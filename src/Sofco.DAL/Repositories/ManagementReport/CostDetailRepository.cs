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

        public List<CostDetailType> GetResourceTypes()
        {
            return context.CostDetailTypes.OrderBy(t => t.Id).ToList();
        }

        public IList<CostDetail> GetByManagementReportAndDates(int managementReportId, DateTime startDate, DateTime endDate)
        {
            return context.CostDetails
                .Include(x => x.CostDetailResources)
                .Include(x => x.CostDetailProfiles)
                .Include(x => x.CostDetailOthers)
                .Where(x => x.ManagementReportId == managementReportId && x.MonthYear.Date >= startDate.Date &&
                            x.MonthYear.Date <= endDate.Date).ToList();
        }
    }
}
