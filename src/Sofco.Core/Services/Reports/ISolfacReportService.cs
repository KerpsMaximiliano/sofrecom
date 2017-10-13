using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Model.Models.Report;

namespace Sofco.Core.Services.Reports
{
    public interface ISolfacReportService
    {
        Result<List<SolfacReport>> Get(DateTime dateSince, DateTime dateTo);
    }
}
