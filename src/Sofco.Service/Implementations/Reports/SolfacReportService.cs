using Sofco.Core.Services.Reports;
using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Core.DAL.Report;
using Sofco.Model.Models.Report;

namespace Sofco.Service.Implementations.Reports
{
    public class SolfacReportService : ISolfacReportService
    {
        private readonly ISolfacReportRepository solfacReportRepository;

        public SolfacReportService(ISolfacReportRepository solfacReportRepository)
        {
            this.solfacReportRepository = solfacReportRepository;
        }
        public Result<List<SolfacReport>> Get(DateTime dateSince, DateTime dateTo)
        {
            return new Result<List<SolfacReport>>(solfacReportRepository.Get());
        }
    }
}
