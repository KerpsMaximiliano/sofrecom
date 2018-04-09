using System.Collections.Generic;
using Sofco.Core.Models.AllocationManagement;

namespace Sofco.Core.Managers
{
    public interface IEmployeeManager
    {
        List<EmployeeWorkTimeApproval> GetByCurrentServices();
    }
}