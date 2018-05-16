using System.Collections.Generic;
using Sofco.Core.Managers;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class WorkTimeApprovalEmployeeService : IWorkTimeApprovalEmployeeService
    {
        private readonly IEmployeeWorkTimeManager employeeWorkTimeManager;

        public WorkTimeApprovalEmployeeService(IEmployeeWorkTimeManager employeeWorkTimeManager)
        {
            this.employeeWorkTimeManager = employeeWorkTimeManager;
        }

        public Response<List<EmployeeWorkTimeApproval>> Get(WorkTimeApprovalQuery query)
        {
            var employees = employeeWorkTimeManager.GetByCurrentServices(query);

            return new Response<List<EmployeeWorkTimeApproval>>{ Data = employees };
        }
    }
}
