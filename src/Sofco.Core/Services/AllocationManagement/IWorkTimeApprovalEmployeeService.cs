using System.Collections.Generic;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IWorkTimeApprovalEmployeeService
    {
        Response<List<EmployeeWorkTimeApproval>> Get(WorkTimeApprovalQuery query);
    }
}