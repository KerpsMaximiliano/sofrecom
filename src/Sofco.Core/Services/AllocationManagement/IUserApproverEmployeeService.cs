using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IUserApproverEmployeeService
    {
        Response<List<UserApproverEmployeeModel>> Get(UserApproverQuery query, UserApproverType type);
        Response<List<ApproverUserDelegate>> GetByUserId(int userId, UserApproverType type);
    }
}