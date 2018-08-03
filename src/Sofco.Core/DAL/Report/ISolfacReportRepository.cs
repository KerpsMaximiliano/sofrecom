using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Models.Reports;

namespace Sofco.Core.DAL.Report
{
    public interface ISolfacReportRepository : IBaseRepository<Solfac>
    {
        List<SolfacReport> Get(SolfacReportParams parameters);
    }
}
