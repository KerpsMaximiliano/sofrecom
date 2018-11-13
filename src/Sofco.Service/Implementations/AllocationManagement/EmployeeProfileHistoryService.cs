using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class EmployeeProfileHistoryService : IEmployeeProfileHistoryService
    {
        private readonly IEmployeeProfileHistoryRepository repository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        public EmployeeProfileHistoryService(IEmployeeProfileHistoryRepository repository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        public Response<List<EmployeeProfileHistoryModel>> GetByEmployeeNumber(string employeeNumber)
        {
            var result = Translate(repository.GetByEmployeeNumber(employeeNumber));

            ResolveManager(result);

            return new Response<List<EmployeeProfileHistoryModel>> { Data = result };
        }

        private void ResolveManager(List<EmployeeProfileHistoryModel> models)
        {
            var managerIds = models
                .Where(s => s.Employee.ManagerId.HasValue)
                .Select(s => s.Employee.ManagerId.Value)
                .ToList();

            managerIds.AddRange(models
                .Where(s => s.EmployeePrevious.ManagerId.HasValue)
                .Select(s => s.EmployeePrevious.ManagerId.Value)
                .ToList());

            managerIds = managerIds.Distinct().ToList();

            var managers = unitOfWork.UserRepository.GetUserLiteByIds(managerIds);

            foreach (var model in models)
            {
                if (model.Employee.ManagerId.HasValue)
                {
                    var userLite = managers.First(s => s.Id == model.Employee.ManagerId.Value);

                    model.Employee.Manager = Translate(userLite);
                }

                if (model.EmployeePrevious.ManagerId.HasValue)
                {
                    var userLite = managers.First(s => s.Id == model.EmployeePrevious.ManagerId.Value);

                    model.EmployeePrevious.Manager = Translate(userLite);
                }
            }
        }

        private List<EmployeeProfileHistoryModel> Translate(List<EmployeeProfileHistory> data)
        {
            return mapper.Map<List<EmployeeProfileHistory>, List<EmployeeProfileHistoryModel>>(data);
        }

        private User Translate(UserLiteModel data)
        {
            return mapper.Map<UserLiteModel, User>(data);
        }
    }
}