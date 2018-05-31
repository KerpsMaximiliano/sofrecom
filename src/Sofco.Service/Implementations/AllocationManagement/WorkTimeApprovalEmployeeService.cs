using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.Managers;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.Models.WorkTimeManagement;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class WorkTimeApprovalEmployeeService : IWorkTimeApprovalEmployeeService
    {
        private readonly IEmployeeWorkTimeManager employeeWorkTimeManager;

        private readonly IMapper mapper;

        public WorkTimeApprovalEmployeeService(IEmployeeWorkTimeManager employeeWorkTimeManager, IMapper mapper)
        {
            this.employeeWorkTimeManager = employeeWorkTimeManager;
            this.mapper = mapper;
        }

        public Response<List<WorkTimeApprovalEmployeeModel>> Get(WorkTimeApprovalQuery query)
        {
            var employees = employeeWorkTimeManager.GetByCurrentServices(query);

            return new Response<List<WorkTimeApprovalEmployeeModel>>{ Data = Translate(employees) };
        }

        private List<WorkTimeApprovalEmployeeModel> Translate(List<WorkTimeApprovalEmployee> data)
        {
            return mapper.Map<List<WorkTimeApprovalEmployee>, List<WorkTimeApprovalEmployeeModel>>(data);
        }
    }
}
