using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.Core.DAL.Common
{
    public interface IUserApproverEmployeeRepository
    {
        List<UserApproverEmployee> Get(UserApproverQuery query, UserApproverType type);

        List<UserApproverEmployee> GetByAnalytics(List<int> analyticIds, int approvalId, UserApproverType type);
    }
}