using System.Collections.Generic;
using Sofco.Domain.Models.Common;

namespace Sofco.Core.Managers.UserApprovers
{
    public interface IRefundApproverEmployeeManager
    {
        List<ApproverUserDelegate> Get(int userId);
    }
}
