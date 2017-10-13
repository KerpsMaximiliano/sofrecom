using System;
using System.Collections.Generic;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.Services.Reports
{
    public interface ISolfacReportService
    {
        List<Solfac> Get(DateTime dateSince, DateTime dateTo);
    }
}
