namespace Sofco.Core.Services.Jobs
{
    public interface IEmployeeSyncActionJobService
    {
        void SyncNewEmployees();

        void SyncEndEmployees();
    }
}
