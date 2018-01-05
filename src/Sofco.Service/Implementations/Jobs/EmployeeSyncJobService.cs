using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.DAL;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Rh.Tiger;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Repository.Rh.Repositories.Interfaces;
using Sofco.Repository.Rh.Settings;

namespace Sofco.Service.Implementations.Jobs
{
    public class EmployeeSyncJobService : IEmployeeSyncJobService
    {
        private const int FromLastMonth = -1;

        private readonly ITigerEmployeeRepository tigerEmployeeRepository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        private readonly DateTime startDate;

        public EmployeeSyncJobService(ITigerEmployeeRepository tigerEmployeeRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.tigerEmployeeRepository = tigerEmployeeRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            startDate = DateTime.UtcNow.AddMonths(FromLastMonth);
        }

        public void SyncNewEmployees()
        {
            var procesedEmployees = Translate(tigerEmployeeRepository.GetWithStartDate(startDate).ToList());

            procesedEmployees = procesedEmployees.Where(s => s.EndDate == null).ToList();

            var newEmployees = GetNewEmployees(procesedEmployees);

            var syncActions = Translate(newEmployees, EmployeeSyncActionStatus.New);

            unitOfWork.EmployeeSyncActionRepository.Save(syncActions);
        }

        private List<Employee> GetNewEmployees(List<Employee> employees)
        {
            var numbers = employees.Select(s => s.EmployeeNumber).ToArray();

            var storedEmployees = unitOfWork.EmployeeRepository.GetByEmployeeNumber(numbers);

            var newEmployees = new List<Employee>();

            foreach (var employee in employees)
            {
                var employeeNumber = employee.EmployeeNumber;

                var isNew = storedEmployees.All(s => s.EmployeeNumber != employeeNumber);

                if (!isNew)
                {
                    var storedEmployee = storedEmployees.FirstOrDefault(s => s.EmployeeNumber == employeeNumber);
                    if (storedEmployee != null 
                        && storedEmployee.StartDate != employee.StartDate)
                    {
                        isNew = true;
                    }
                }

                if (isNew)
                {
                    newEmployees.Add(employee);
                }
            }

            return newEmployees;
        }

        public void SyncEndEmployees()
        {
            var procesedEmployees = Translate(tigerEmployeeRepository.GetWithEndDate(startDate).ToList());

            var endEmployees = GetEndEmployees(procesedEmployees);

            var syncActions = Translate(endEmployees, EmployeeSyncActionStatus.Delete);

            unitOfWork.EmployeeSyncActionRepository.Save(syncActions);
        }

        private List<Employee> GetEndEmployees(List<Employee> employees)
        {
            var numbers = employees.Select(s => s.EmployeeNumber).ToArray();

            var storedEmployees = unitOfWork.EmployeeRepository.GetByEmployeeNumber(numbers);

            var endEmployees = new List<Employee>();

            foreach (var employee in employees)
            {
                var employeeNumber = employee.EmployeeNumber;

                var storedEmployee = storedEmployees.FirstOrDefault(s => s.EmployeeNumber == employeeNumber);
                if (storedEmployee != null 
                    && storedEmployee.EndDate == null
                    && employee.EndDate > RhSetting.TigerDateTimeMinValue)
                {
                    endEmployees.Add(employee);
                }
            }

            return endEmployees;
        }

        private List<Employee> Translate(List<TigerEmployee> tigerEmployees)
        {
            return mapper.Map<List<TigerEmployee>, List<Employee>>(tigerEmployees);
        }

        private List<EmployeeSyncAction> Translate(List<Employee> employees, string status)
        {
            var result = mapper.Map<List<Employee>, List<EmployeeSyncAction>>(employees);

            result.ForEach(s => { s.Status = status; });

            return result;
        }
    }
}
