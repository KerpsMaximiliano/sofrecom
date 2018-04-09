using System.Collections.Generic;
using Sofco.Core.Managers;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class EmployeeWorkTimeApprovalService : IEmployeeWorkTimeApprovalService
    {
        private readonly IEmployeeManager employeeManager;

        public EmployeeWorkTimeApprovalService(IEmployeeManager employeeManager)
        {
            this.employeeManager = employeeManager;
        }

        public Response<List<EmployeeWorkTimeApproval>> Get()
        {
            var employees = employeeManager.GetByCurrentServices();

            return new Response<List<EmployeeWorkTimeApproval>>{ Data = employees };
        }
    }
}
