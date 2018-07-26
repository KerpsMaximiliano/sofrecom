using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class CustomerUpdateJob : ICustomerUpdateJob
    {
        private readonly ICustomerUpdateJobService service;

        public CustomerUpdateJob(ICustomerUpdateJobService service)
        {
            this.service = service;
        }

        public void Execute()
        {
            service.Execute();
        }
    }
}
