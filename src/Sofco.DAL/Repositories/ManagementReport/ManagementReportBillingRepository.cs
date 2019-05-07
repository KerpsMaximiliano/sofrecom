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
                  .Include(x => x.ManagementReport.CostDetails)
                  .Include(x => x.ManagementReport.Billings)
                .FirstOrDefault();

            return data;
        }
    }
}
