using System.Collections.Generic;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeProfileHistoryRepository
    {
        void Save(EmployeeProfileHistory employeeProfileHistory);

        List<EmployeeProfileHistory> GetByEmployeeNumber(string employeeNumber);
    }
}