using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Models.Common;

namespace Sofco.Core.Managers.UserApprovers
{
    public interface ILicenseApproverEmployeeManager
    {
        List<UserApproverEmployee> Get(UserApproverQuery query);
    }
}