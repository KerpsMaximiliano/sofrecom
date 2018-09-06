using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.Core.Managers
{
    public interface IEmployeeWorkTimeManager
    {
        List<UserApproverEmployee> GetByCurrentServices(UserApproverQuery query, UserApproverType type);
    }
}