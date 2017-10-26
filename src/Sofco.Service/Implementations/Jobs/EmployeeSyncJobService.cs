using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Rh.Tiger;
using Sofco.Domain.Rh.Rhpro;
using Sofco.Model.Models.TimeManagement;
using Sofco.Repository.Rh.Repositories.Interfaces;

namespace Sofco.Service.Implementations.Jobs
{
    public class EmployeeSyncJobService : IEmployeeSyncJobService
    {
        const int FromLastYears = -1;

        private readonly ITigerEmployeeRepository tigerEmployeeRepository;

        private readonly IRhproEmployeeRepository rhproEmployeeRepository;

        private readonly IEmployeeRepository employeeRepository;

        private readonly ILicenseTypeRepository licenseTypeRepository;

        private readonly IEmployeeLicenseRepository employeeLicenseRepository;

        private readonly IMapper mapper;

        private readonly DateTime startDate;

        public EmployeeSyncJobService(ITigerEmployeeRepository tigerEmployeeRepository,
            IRhproEmployeeRepository rhproEmployeeRepository,
            IEmployeeRepository employeeRepository,
            ILicenseTypeRepository licenseTypeRepository,
            IEmployeeLicenseRepository employeeLicenseRepository,
            IMapper mapper)
        {
            this.tigerEmployeeRepository = tigerEmployeeRepository;
            this.rhproEmployeeRepository = rhproEmployeeRepository;
            this.employeeRepository = employeeRepository;
            this.licenseTypeRepository = licenseTypeRepository;
            this.employeeLicenseRepository = employeeLicenseRepository;
            this.mapper = mapper;
            startDate = DateTime.UtcNow.AddYears(FromLastYears);
        }

        public void Sync()
        {
            SyncLicenseType();

            SyncEmployeeLicense();

            SyncEmployee();
        }

        private void SyncLicenseType()
        {
            var licenseTypes = Translate(rhproEmployeeRepository.GetLicenseTypes().ToList());

            licenseTypeRepository.Save(licenseTypes);
        }

        private void SyncEmployeeLicense()
        {
            var employeeLicenses = Translate(rhproEmployeeRepository.GetEmployeeLicensesWithStartDate(startDate).ToList());

            employeeLicenseRepository.Delete(employeeLicenses, startDate);

            employeeLicenseRepository.Save(employeeLicenses);
        }

        private void SyncEmployee()
        {
            var employees = Translate(tigerEmployeeRepository.GetWithStartDate(startDate).ToList());

            employeeRepository.Save(employees);
        }

        private List<Employee> Translate(List<TigerEmployee> tigerEmployees)
        {
            return mapper.Map<List<TigerEmployee>, List<Employee>>(tigerEmployees);
        }

        private List<LicenseType> Translate(List<RhproLicenseType> rhproLicenseType)
        {
            return mapper.Map<List<RhproLicenseType>, List<LicenseType>>(rhproLicenseType);
        }

        private List<EmployeeLicense> Translate(List<RhproEmployeeLicense> rhproEmployeeLicense)
        {
            return mapper.Map<List<RhproEmployeeLicense>, List<EmployeeLicense>>(rhproEmployeeLicense);
        }
    }
}
