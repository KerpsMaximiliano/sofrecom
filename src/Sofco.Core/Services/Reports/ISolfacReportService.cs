using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Domain.Models.Reports;

namespace Sofco.Core.Services.Reports
{
    public interface ISolfacReportService
    {
        Result<List<SolfacReport>> Get(SolfacReportParams parameters);
    }
}
