using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.ManagementReport;

namespace Sofco.DAL.Repositories.ManagementReport
{
    public class ManagementReportBillingRepository : BaseRepository<ManagementReportBilling>, IManagementReportBillingRepository
    {
        public ManagementReportBillingRepository(SofcoContext context) : base(context)
        {
        }
    }
}
