using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IWorkTimeApprovalEmployeeService
    {
        Response<List<WorkTimeApprovalEmployeeModel>> Get(WorkTimeApprovalQuery query);
    }
}