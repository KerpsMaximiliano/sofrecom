namespace Sofco.Core.Services.Jobs
{
    public interface IEmployeeSyncJobService
    {
        void SyncNewEmployees();

        void SyncEndEmployees();
    }
}
