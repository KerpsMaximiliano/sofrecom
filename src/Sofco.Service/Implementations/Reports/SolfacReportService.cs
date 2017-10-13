using Sofco.Core.Services.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sofco.Model.Models.Billing;
using Sofco.Core.DAL.Billing;

namespace Sofco.Service.Implementations.Reports
{
    public class SolfacReportService : ISolfacReportService
    {
        private readonly ISolfacRepository solfacRepository;

        public SolfacReportService(ISolfacRepository solfacRepository)
        {
            this.solfacRepository = solfacRepository;
        }
        public List<Solfac> Get(DateTime dateSince, DateTime dateTo)
        {
            throw new NotImplementedException();
        }
    }
}
