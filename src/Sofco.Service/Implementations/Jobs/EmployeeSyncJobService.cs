﻿using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.DAL;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Rh.Tiger;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Repository.Rh.Repositories.Interfaces;

namespace Sofco.Service.Implementations.Jobs
{
    public class EmployeeSyncJobService : IEmployeeSyncJobService
    {
        const int FromLastYears = -1;

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
            startDate = DateTime.UtcNow.AddYears(FromLastYears);
        }

        public void Sync()
        {
            SyncEmployee();
        }

        private void SyncEmployee()
        {
            var employees = Translate(tigerEmployeeRepository.GetWithStartDate(startDate).ToList());

            unitOfWork.EmployeeRepository.Save(employees);
        }

        private List<Employee> Translate(List<TigerEmployee> tigerEmployees)
        {
            return mapper.Map<List<TigerEmployee>, List<Employee>>(tigerEmployees);
        }
    }
}
