using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.Core.DAL.Common
{
    public interface IUserApproverEmployeeRepository
    {
        List<UserApproverEmployee> GetByAllocations(UserApproverQuery query, UserApproverType type);

        List<UserApproverEmployee> GetByAllocationAnalytics(List<int> analyticIds, int approvalId, UserApproverType type);

        List<UserApproverEmployee> GetByAllocationManagersBySectors(List<int> sectorIds, UserApproverType type);

        List<UserApproverEmployee> GetByManager(int managerId, UserApproverQuery query, UserApproverType type);
    }
}