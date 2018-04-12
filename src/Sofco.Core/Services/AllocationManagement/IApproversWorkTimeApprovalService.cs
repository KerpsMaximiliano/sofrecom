using System.Collections.Generic;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IApproversWorkTimeApprovalService
    {
        Response<List<UserSelectListItem>> GetApprovers(WorkTimeApprovalQuery query);
    }
}