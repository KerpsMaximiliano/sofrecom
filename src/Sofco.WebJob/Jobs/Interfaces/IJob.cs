using Hangfire;

namespace Sofco.WebJob.Jobs.Interfaces
{
    public interface IJob
    {
        [DisableConcurrentExecution(20)]
        void Execute();
    }
}
