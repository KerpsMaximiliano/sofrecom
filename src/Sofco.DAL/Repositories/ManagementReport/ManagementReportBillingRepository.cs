using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.ManagementReport;
using System.Linq;

namespace Sofco.DAL.Repositories.ManagementReport
{
    public class ManagementReportBillingRepository : BaseRepository<ManagementReportBilling>, IManagementReportBillingRepository
    {
        public ManagementReportBillingRepository(SofcoContext context) : base(context)
        {
        }

        public ManagementReportBilling GetById(int IdManamentReport)
        {
            var data = context.ManagementReportBillings
                .Where(mr => mr.ManagementReportId == IdManamentReport)
                .FirstOrDefault();

            return data;
        }

        public IList<ManagementReportBilling> GetByManagementReportAndDates(int managementReportId, DateTime startDate, DateTime endDate)
        {
            return context.ManagementReportBillings.Where(x =>
                x.ManagementReportId == managementReportId && x.MonthYear.Date >= startDate.Date &&
                x.MonthYear.Date <= endDate.Date).ToList();
        }

        public ManagementReportBilling GetByManagementReportIdAndDate(int managementReportId, DateTime monthYear)
        {
            return context.ManagementReportBillings.SingleOrDefault(x =>
                x.ManagementReportId == managementReportId && x.MonthYear.Date == monthYear.Date);
        }
    }
}
