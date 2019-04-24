using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class ContactUpdateJob : IContactUpdateJob
    {
        private readonly IContactUpdateJobService service;

        public ContactUpdateJob(IContactUpdateJobService service)
        {
            this.service = service;
        }

        public void Execute()
        {
            service.Execute();
        }
    }
}
