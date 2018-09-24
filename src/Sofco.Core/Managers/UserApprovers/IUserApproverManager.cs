using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.Core.Managers.UserApprovers
{
    public interface IUserApproverManager
    {
        List<UserApproverEmployee> GetByCurrentManager(UserApproverQuery query, UserApproverType type);

        List<UserApproverEmployee> ResolveData(List<UserApproverEmployee> data);

        bool IsDirector();

        bool IsManager();
    }
}