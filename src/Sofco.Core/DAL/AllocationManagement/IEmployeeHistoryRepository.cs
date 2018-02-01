using Sofco.Core.DAL.Common;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeHistoryRepository : IBaseRepository<EmployeeHistory>
    {
        void Save(EmployeeHistory employeeHistory);
    }
}