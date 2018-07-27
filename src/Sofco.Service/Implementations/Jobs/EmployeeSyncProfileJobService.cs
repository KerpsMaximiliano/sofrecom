using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Rh.Tiger;
using Sofco.Framework.Helpers;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Repository.Rh.Repositories.Interfaces;

namespace Sofco.Service.Implementations.Jobs
{
    public class EmployeeSyncProfileJobService : IEmployeeSyncProfileJobService
    {
        private readonly ITigerEmployeeRepository tigerEmployeeRepository;

        private readonly IEmployeeProfileHistoryRepository employeeProfileHistoryRepository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        public EmployeeSyncProfileJobService(ITigerEmployeeRepository tigerEmployeeRepository, IUnitOfWork unitOfWork, IMapper mapper, IEmployeeProfileHistoryRepository employeeProfileHistoryRepository)
        {
            this.tigerEmployeeRepository = tigerEmployeeRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.employeeProfileHistoryRepository = employeeProfileHistoryRepository;
        }

        public void Sync()
        {
            var procesedEmployees = Translate(tigerEmployeeRepository.GetActive());

            TrimStringFields(procesedEmployees);

            var employees = unitOfWork.EmployeeRepository.GetAll();

            foreach (var stored in employees)
            {
                var newEmployee = procesedEmployees.FirstOrDefault(s => s.EmployeeNumber == stored.EmployeeNumber);
                if(newEmployee == null) continue;

                var modifiedFields = ElementComparerHelper.CompareModification(newEmployee, stored, GetFieldToCompare());
                if (!modifiedFields.Any()) continue;

                employeeProfileHistoryRepository.Save(CreateProfileHistory(stored, newEmployee, modifiedFields));

                ElementComparerHelper.ApplyModifications(stored, newEmployee, modifiedFields);

                unitOfWork.EmployeeRepository.Save(stored);
            }
        }

        private string[] GetFieldToCompare()
        {
            return new[]
            {
                nameof(Employee.Name),
                nameof(Employee.Birthday),
                nameof(Employee.Profile),
                nameof(Employee.Technology),
                nameof(Employee.Seniority),
                nameof(Employee.Address),
                nameof(Employee.Location),
                nameof(Employee.Province),
                nameof(Employee.Country),
                nameof(Employee.HealthInsuranceCode),
                nameof(Employee.PrepaidHealthCode)
            };
        }

        private void TrimStringFields(List<Employee> employees)
        {
            foreach (var employee in employees)
            {
                employee.Name = employee.Name?.Trim();
                employee.Technology = employee.Technology?.Trim();
                employee.Seniority = employee.Seniority?.Trim();
            }
        }

        private EmployeeProfileHistory CreateProfileHistory(Employee current, Employee previous, string[] modifiedFields)
        {
            var currentData = JsonSerializeHelper.Serialize(current);
            var previousData = JsonSerializeHelper.Serialize(previous);
            var modifiedFieldsData = JsonSerializeHelper.Serialize(modifiedFields);

            return new EmployeeProfileHistory
            {
                Created = DateTime.UtcNow,
                EmployeeNumber = current.EmployeeNumber,
                EmployeeData = currentData,
                EmployeePreviousData = previousData,
                ModifiedFields = modifiedFieldsData
            };
        }

        private List<Employee> Translate(List<TigerEmployee> tigerEmployees)
        {
            return mapper.Map<List<TigerEmployee>, List<Employee>>(tigerEmployees);
        }
    }
}
