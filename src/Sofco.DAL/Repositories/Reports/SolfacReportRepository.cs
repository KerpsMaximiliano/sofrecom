using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Report;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Models.Reports;

namespace Sofco.DAL.Repositories.Reports
{
    public class SolfacReportRepository : BaseRepository<Solfac>, ISolfacReportRepository
    {
        public SolfacReportRepository(SofcoContext context) : base(context)
        {
        }

        public List<SolfacReport> Get(SolfacReportParams parameters)
        {
            var dateSince = parameters.DateSince;
            var dateTo = parameters.DateTo;

            return context.Solfacs
                .Where(s => 
                s.InvoiceDate >= dateSince
                && s.InvoiceDate <= dateTo)
                .Select(x => new SolfacReport
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    ProjectName = x.Project,
                    BusinessName = x.BusinessName,
                    AccountName = x.AccountName,
                    InvoiceCode = x.InvoiceCode,
                    InvoiceDate = x.InvoiceDate.Value,
                    Amount = x.TotalAmount
                }).OrderBy(x => x.InvoiceDate).ToList();
        }
    }
}
