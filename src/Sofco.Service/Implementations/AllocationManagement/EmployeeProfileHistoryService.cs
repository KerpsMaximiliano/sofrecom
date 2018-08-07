using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class EmployeeProfileHistoryService : IEmployeeProfileHistoryService
    {
        private readonly IEmployeeProfileHistoryRepository repository;

        private readonly IMapper mapper;

        private readonly IEmployeeData employeeData;

        public EmployeeProfileHistoryService(IEmployeeProfileHistoryRepository repository, IMapper mapper, IEmployeeData employeeData)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.employeeData = employeeData;
        }

        public Response<List<EmployeeProfileHistoryModel>> GetByCurrentUser()
        {
            var currentEmployee = employeeData.GetCurrentEmployee();

            var result = Translate(repository.GetByEmployeeNumber(currentEmployee.EmployeeNumber));

            return new Response<List<EmployeeProfileHistoryModel>> { Data = result };
        }

        private List<EmployeeProfileHistoryModel> Translate(List<EmployeeProfileHistory> data)
        {
            return mapper.Map<List<EmployeeProfileHistory>, List<EmployeeProfileHistoryModel>>(data);
        }
    }
}
