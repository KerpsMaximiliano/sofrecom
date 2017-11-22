using Sofco.Core.DAL.Common;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface ICostCenterRepository : IBaseRepository<CostCenter>
    {
        bool ExistCode(int domainCode);
    }
}
