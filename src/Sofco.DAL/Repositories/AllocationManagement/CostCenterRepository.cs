using System.Linq;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class CostCenterRepository : BaseRepository<CostCenter>, ICostCenterRepository
    {
        public CostCenterRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistCode(int domainCode)
        {
            return context.CostCenters.Any(x => x.Code == domainCode);
        }
    }
}
