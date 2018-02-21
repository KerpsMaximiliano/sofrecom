using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class EmployeeForceSaveJob : IEmployeeForceSaveJob
    {
        private readonly IEmployeeForceSaveSyncJobService service;

        public EmployeeForceSaveJob(IEmployeeForceSaveSyncJobService service)
        {
            this.service = service;
        }

        public void Execute()
        {
            service.SyncSave();
        }
    }
}
