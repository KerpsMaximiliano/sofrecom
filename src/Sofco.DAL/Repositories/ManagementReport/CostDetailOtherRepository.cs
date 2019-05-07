using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.ManagementReport;

namespace Sofco.DAL.Repositories.ManagementReport
{
    public class CostDetailOtherRepository : BaseRepository<CostDetailOther>, ICostDetailOtherRepository
    {
        public CostDetailOtherRepository(SofcoContext context) : base(context)
        {
        }
    }
}
