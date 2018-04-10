using System.Collections.Generic;
using Sofco.Core.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeWorkTimeApprovalRepository
    {
        List<EmployeeWorkTimeApproval> Get();
    }
}