using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.Billing;
using Sofco.Model.Models.Report;

namespace Sofco.Core.DAL.Report
{
    public interface ISolfacReportRepository : IBaseRepository<Solfac>
    {
        List<SolfacReport> Get();
    }
}
