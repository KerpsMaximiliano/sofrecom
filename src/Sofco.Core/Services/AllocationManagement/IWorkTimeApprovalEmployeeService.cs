using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IWorkTimeApprovalEmployeeService
    {
        Response<List<WorkTimeApprovalEmployeeModel>> Get(WorkTimeApprovalQuery query);
    }
}