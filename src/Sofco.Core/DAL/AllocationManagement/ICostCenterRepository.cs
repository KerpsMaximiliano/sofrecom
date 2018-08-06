using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface ICostCenterRepository : IBaseRepository<CostCenter>
    {
        bool ExistCode(int domainCode);
    }
}
