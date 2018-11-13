using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeProfileHistoryRepository : IBaseRepository<EmployeeProfileHistory>
    {
        void Save(EmployeeProfileHistory employeeProfileHistory);

        List<EmployeeProfileHistory> GetByEmployeeNumber(string employeeNumber);
    }
}