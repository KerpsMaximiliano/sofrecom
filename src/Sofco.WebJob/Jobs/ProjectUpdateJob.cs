using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class ProjectUpdateJob : IProjectUpdateJob
    {
        private readonly IProjectUpdateJobService service;

        public ProjectUpdateJob(IProjectUpdateJobService service)
        {
            this.service = service;
        }

        public void Execute()
        {
            service.Execute();
        }
    }
}
