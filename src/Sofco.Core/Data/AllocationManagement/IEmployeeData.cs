using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.Data.AllocationManagement
{
    public interface IEmployeeData
    {
        Employee GetByEmail(string email);
    }
}