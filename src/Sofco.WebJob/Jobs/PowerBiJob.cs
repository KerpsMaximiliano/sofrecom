using System;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.DTO;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class PowerBiJob : IPowerBiJob
    {
        private readonly IAllocationService service;

        public PowerBiJob(IAllocationService service)
        {
            this.service = service;
        }

        public void Execute()
        {
            var parameters = new AllocationReportParams();

            parameters.Unassigned = true;
            parameters.GenerateReportPowerBi = true;
            parameters.StartDate = DateTime.UtcNow;
            parameters.EndDate = DateTime.UtcNow.AddMonths(4);

            this.service.CreateReport(parameters);
        }
    }
}
