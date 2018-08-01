using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Core.DAL;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Rh.Tiger;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Repository.Rh.Repositories.Interfaces;

namespace Sofco.Service.Implementations.Jobs
{ 
    public class EmployeeForceSaveSyncJobService : IEmployeeForceSaveSyncJobService
    {
        private readonly DateTime startDate;

        private readonly ITigerEmployeeRepository tigerEmployeeRepository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        public EmployeeForceSaveSyncJobService(ITigerEmployeeRepository tigerEmployeeRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.tigerEmployeeRepository = tigerEmployeeRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            startDate = DateTime.UtcNow.AddYears(-100);
        }

        public void SyncSave()
        {
            var employees = Translate(tigerEmployeeRepository.GetWithStartDate(startDate).ToList());

            unitOfWork.EmployeeRepository.Update(employees);
        }

        public void AddEmployee()
        {
            var employees = Translate(tigerEmployeeRepository.GetByLegajs(new []{ 1003 }).ToList());

            unitOfWork.EmployeeRepository.Save(employees);
        }

        private List<Employee> Translate(List<TigerEmployee> tigerEmployees)
        {
            return mapper.Map<List<TigerEmployee>, List<Employee>>(tigerEmployees);
        }
    }
}
