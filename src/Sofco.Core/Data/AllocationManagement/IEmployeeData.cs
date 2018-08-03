using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.Data.AllocationManagement
{
    public interface IEmployeeData
    {
        Employee GetCurrentEmployee();
    }
}