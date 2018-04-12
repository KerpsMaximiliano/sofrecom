using System.Collections.Generic;
using Sofco.Core.Models.AllocationManagement;

namespace Sofco.Core.Managers
{
    public interface IEmployeeWorkTimeManager
    {
        List<EmployeeWorkTimeApproval> GetByCurrentServices(WorkTimeApprovalQuery query);
    }
}