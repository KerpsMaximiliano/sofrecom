using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Core.DAL;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Rh.Rhpro;
using Sofco.Domain.Rh.Tiger;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Repository.Rh.Repositories.Interfaces;

namespace Sofco.Service.Implementations.Jobs
{
    public class EmployeeLicenseSyncJobService : IEmployeeLicenseSyncJobService
    {
        const int FromLastYears = -1;

        private readonly ITigerEmployeeRepository tigerEmployeeRepository;

        private readonly IRhproEmployeeRepository rhproEmployeeRepository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        private readonly DateTime startDate;

        public EmployeeLicenseSyncJobService(ITigerEmployeeRepository tigerEmployeeRepository,
            IRhproEmployeeRepository rhproEmployeeRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.tigerEmployeeRepository = tigerEmployeeRepository;
            this.rhproEmployeeRepository = rhproEmployeeRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            startDate = DateTime.UtcNow.AddYears(FromLastYears);
        }

        public void Sync()
        {
            //SyncLicenseType();

            //SyncEmployee();
        }

        //private void SyncLicenseType()
        //{
        //    var licenseTypes = Translate(rhproEmployeeRepository.GetLicenseTypes().ToList());

        //    unitOfWork.LicenseTypeRepository.Save(licenseTypes);
        //}

        //private void SyncEmployee()
        //{
        //    var employees = Translate(tigerEmployeeRepository.GetWithStartDate(startDate).ToList());

        //    unitOfWork.EmployeeRepository.Save(employees);
        //}

        //private List<Employee> Translate(List<TigerEmployee> tigerEmployees)
        //{
        //    return mapper.Map<List<TigerEmployee>, List<Employee>>(tigerEmployees);
        //}

        //private List<LicenseType> Translate(List<RhproLicenseType> rhproLicenseType)
        //{
        //    return mapper.Map<List<RhproLicenseType>, List<LicenseType>>(rhproLicenseType);
        //}
    }
}
