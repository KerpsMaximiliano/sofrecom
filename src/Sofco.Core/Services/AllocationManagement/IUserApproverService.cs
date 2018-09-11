using System.Collections.Generic;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IUserApproverService
    {
        Response<List<UserApproverModel>> Save(List<UserApproverModel> userApprovers, UserApproverType type);

        Response Delete(int workTimeApprovalId);

        Response<List<UserSelectListItem>> GetApprovers(UserApproverQuery query, UserApproverType type);
    }
}