using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Core.DAL;
using Sofco.Core.Services.Reports;
using Sofco.Model.Models.Reports;

namespace Sofco.Service.Implementations.Reports
{
    public class SolfacReportService : ISolfacReportService
    {
        private readonly IUnitOfWork unitOfWork;

        public SolfacReportService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public Result<List<SolfacReport>> Get(SolfacReportParams parameters)
        {
            return new Result<List<SolfacReport>>(unitOfWork.SolfacReportRepository.Get(parameters));
        }
    }
}
