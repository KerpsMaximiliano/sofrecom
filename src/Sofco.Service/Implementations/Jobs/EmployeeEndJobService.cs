using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Services.Jobs;
using System;
using System.Linq;

namespace Sofco.Service.Implementations.Jobs
{
    public class EmployeeEndJobService : IEmployeeEndJobService
    {
        private readonly IEmployeeRepository employeeRepository;

        public EmployeeEndJobService(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        public void SendNotification()
        {
            var today = DateTime.Now;

            var employeeEnds = employeeRepository.GetByEndDate(today);

            if(employeeEnds.Any())
            {
                //SEND NOTIFICATIONS
            }
        }
    }
}
