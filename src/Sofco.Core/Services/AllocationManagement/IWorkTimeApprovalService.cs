using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IWorkTimeApprovalService
    {
        Response<List<WorkTimeApproval>> GetAll();

        Response<List<WorkTimeApproval>> Save(List<WorkTimeApproval> workTimeApprovals);

        Response Delete(int workTimeApprovalId);
    }
}