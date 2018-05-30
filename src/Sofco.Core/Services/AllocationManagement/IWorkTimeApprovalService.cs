using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IWorkTimeApprovalService
    {
        Response<List<WorkTimeApproval>> GetAll();

        Response<List<WorkTimeApprovalModel>> Save(List<WorkTimeApprovalModel> workTimeApprovals);

        Response Delete(int workTimeApprovalId);
    }
}