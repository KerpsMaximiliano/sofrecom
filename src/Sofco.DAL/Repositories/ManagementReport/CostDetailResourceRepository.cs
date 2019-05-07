using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.ManagementReport;

namespace Sofco.DAL.Repositories.ManagementReport
{
    public class CostDetailResourceRepository : BaseRepository<CostDetailResource>, ICostDetailResourceRepository
    {
        public CostDetailResourceRepository(SofcoContext context) : base(context)
        {
        }
    }
}
