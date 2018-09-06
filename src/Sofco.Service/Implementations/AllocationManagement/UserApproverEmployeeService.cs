using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.Managers;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class UserApproverEmployeeService : IUserApproverEmployeeService
    {
        private readonly IEmployeeWorkTimeManager employeeWorkTimeManager;

        private readonly IMapper mapper;

        public UserApproverEmployeeService(IEmployeeWorkTimeManager employeeWorkTimeManager, IMapper mapper)
        {
            this.employeeWorkTimeManager = employeeWorkTimeManager;
            this.mapper = mapper;
        }

        public Response<List<UserApproverEmployeeModel>> Get(UserApproverQuery query, UserApproverType type)
        {
            var employees = employeeWorkTimeManager.GetByCurrentServices(query, type);

            return new Response<List<UserApproverEmployeeModel>>{ Data = Translate(employees) };
        }

        private List<UserApproverEmployeeModel> Translate(List<UserApproverEmployee> data)
        {
            return mapper.Map<List<UserApproverEmployee>, List<UserApproverEmployeeModel>>(data);
        }
    }
}
