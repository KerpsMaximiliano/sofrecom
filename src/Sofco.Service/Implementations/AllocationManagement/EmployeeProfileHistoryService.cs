using System.Collections.Generic;
using AutoMapper;
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

        public EmployeeProfileHistoryService(IEmployeeProfileHistoryRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public Response<List<EmployeeProfileHistoryModel>> GetByEmployeeNumber(string employeeNumber)
        {
            var result = Translate(repository.GetByEmployeeNumber(employeeNumber));

            return new Response<List<EmployeeProfileHistoryModel>> { Data = result };
        }

        private List<EmployeeProfileHistoryModel> Translate(List<EmployeeProfileHistory> data)
        {
            return mapper.Map<List<EmployeeProfileHistory>, List<EmployeeProfileHistoryModel>>(data);
        }
    }
}