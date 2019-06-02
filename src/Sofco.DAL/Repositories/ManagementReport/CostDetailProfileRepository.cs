using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.ManagementReport;

namespace Sofco.DAL.Repositories.ManagementReport
{
    public class CostDetailProfileRepository : BaseRepository<CostDetailProfile>, ICostDetailProfileRepository
    {
        public CostDetailProfileRepository(SofcoContext context) : base(context)
        {
        }
    }
}
