using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Model.Models.WorkTimeManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeWorkTimeApprovalRepository
    {
        List<WorkTimeApprovalEmployee> Get(WorkTimeApprovalQuery query);

        List<WorkTimeApprovalEmployee> GetByAnalytics(List<int> analyticIds, int approvalId);
    }
}