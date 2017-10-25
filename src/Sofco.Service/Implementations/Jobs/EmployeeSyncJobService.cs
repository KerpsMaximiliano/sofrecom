using Sofco.Core.Services.Jobs;
using Sofco.Repository.Rh.Repositories.Interfaces;
using System;
using System.Linq;

namespace Sofco.Service.Implementations.Jobs
{
    public class EmployeeSyncJobService : IEmployeeSyncJobService
    {
        private readonly ITigerEmployeeRepository tigerEmployeeRepository;

        private readonly IRhproEmployeeRepository rhproEmployeeRepository;

        public EmployeeSyncJobService(ITigerEmployeeRepository tigerEmployeeRepository,
            IRhproEmployeeRepository rhproEmployeeRepository)
        {
            this.tigerEmployeeRepository = tigerEmployeeRepository;
            this.rhproEmployeeRepository = rhproEmployeeRepository;
        }

        public void Sync()
        {
            SyncNewEmployee();
        }

        private void SyncNewEmployee()
        {
            var lastYear = DateTime.UtcNow.AddYears(-1);

            var tigerEmployee = tigerEmployeeRepository.GetWithStartDate(lastYear);

            var employeeNumbers = tigerEmployee.Select(s => s.Legaj).ToList();
        }

        private void SyncDeletedEmployee()
        {
            var lastYear = DateTime.UtcNow.AddYears(-1);

            var tigerEmployeeEnds = tigerEmployeeRepository.GetWithEndDate(lastYear);
        }
    }
}
