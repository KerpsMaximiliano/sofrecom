using System.Collections.Generic;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IWorkTimeApprovalApproverService
    {
        Response<List<UserSelectListItem>> GetApprovers(WorkTimeApprovalQuery query);
    }
}