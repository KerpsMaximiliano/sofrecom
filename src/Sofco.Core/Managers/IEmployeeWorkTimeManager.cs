using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Core.Managers
{
    public interface IEmployeeWorkTimeManager
    {
        List<WorkTimeApprovalEmployee> GetByCurrentServices(WorkTimeApprovalQuery query);
    }
}