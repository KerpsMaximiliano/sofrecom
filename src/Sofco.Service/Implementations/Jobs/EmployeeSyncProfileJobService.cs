using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Rh.Tiger;
using Sofco.Framework.Helpers;
using Sofco.Domain.Models.AllocationManagement;
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

        public void Sync(bool all)
        {
            var procesedEmployees = Translate(tigerEmployeeRepository.GetActive());

            TrimStringFields(procesedEmployees);

            var employees = unitOfWork.EmployeeRepository.GetAll();

            foreach (var stored in employees)
            {
                var newEmployee = procesedEmployees.FirstOrDefault(s => s.EmployeeNumber == stored.EmployeeNumber);
                if(newEmployee == null) continue;

                var fields = GetFieldToCompare(all);

                if (string.IsNullOrEmpty(stored.Email))
                {
                    fields.Add(nameof(Employee.Email));
                }

                var modifiedFields = ElementComparerHelper.CompareModification(newEmployee, stored, fields.ToArray());
                if (!modifiedFields.Any()) continue;

                if(all) employeeProfileHistoryRepository.Save(CreateProfileHistory(stored, newEmployee, modifiedFields));

                ElementComparerHelper.ApplyModifications(stored, newEmployee, modifiedFields);

                unitOfWork.EmployeeRepository.Save(stored);
            }
        }

        private List<string> GetFieldToCompare(bool all)
        {
            if (!all)
            {
                return new List<string>
                {
                    nameof(Employee.Salary),
                    nameof(Employee.PrepaidAmount),
                    nameof(Employee.BeneficiariesCount)
                };
            }

            return new List<string>
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
                nameof(Employee.PrepaidHealthCode),
                nameof(Employee.DocumentNumberType),
                nameof(Employee.DocumentNumber),
                nameof(Employee.Cuil),
                nameof(Employee.PhoneCountryCode),
                nameof(Employee.PhoneAreaCode),
                nameof(Employee.PhoneNumber),
                nameof(Employee.Bank),
                nameof(Employee.Salary),
                nameof(Employee.PrepaidAmount),
                nameof(Employee.BeneficiariesCount),
                nameof(Employee.Nationality),
                nameof(Employee.Activity),
                nameof(Employee.ContractType),
            };
        }

        private void TrimStringFields(List<Employee> employees)
        {
            foreach (var employee in employees)
            {
                employee.Name = employee.Name?.Trim();
                employee.Technology = employee.Technology?.Trim();
                employee.Seniority = employee.Seniority?.Trim();
                employee.DocumentNumberType = employee.DocumentNumberType?.Trim();
                employee.PhoneNumber = employee.PhoneNumber?.Trim();
                employee.Email = employee.Email?.Trim();
                employee.Bank = employee.Bank?.Trim();
            }
        }

        private EmployeeProfileHistory CreateProfileHistory(Employee current, Employee updated, string[] modifiedFields)
        {
            var currentData = JsonSerializeHelper.Serialize(current);
            var updateData = JsonSerializeHelper.Serialize(updated);
            var modifiedFieldsData = JsonSerializeHelper.Serialize(modifiedFields);

            return new EmployeeProfileHistory
            {
                Created = DateTime.UtcNow,
                EmployeeNumber = current.EmployeeNumber,
                EmployeeData = updateData,
                EmployeePreviousData = currentData,
                ModifiedFields = modifiedFieldsData
            };
        }

        private List<Employee> Translate(List<TigerEmployee> tigerEmployees)
        {
            return mapper.Map<List<TigerEmployee>, List<Employee>>(tigerEmployees);
        }
        
        private void ProcessEmailCase(Employee oldItem, Employee newItem)
        {
            if (string.IsNullOrEmpty(newItem.Email)
                && !string.IsNullOrEmpty(oldItem.Email))
            {
                newItem.Email = oldItem.Email;
            }
        }
    }
}
