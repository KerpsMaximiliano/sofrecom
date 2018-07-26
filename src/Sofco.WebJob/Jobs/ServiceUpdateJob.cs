using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class ServiceUpdateJob : IServiceUpdateJob
    {
        private readonly IServiceUpdateJobService service;

        public ServiceUpdateJob(IServiceUpdateJobService service)
        {
            this.service = service;
        }

        public void Execute()
        {
            service.Execute();
        }
    }
}
