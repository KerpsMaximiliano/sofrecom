using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeProfileHistoryRepository
    {
        void Save(EmployeeProfileHistory employeeProfileHistory);
    }
}