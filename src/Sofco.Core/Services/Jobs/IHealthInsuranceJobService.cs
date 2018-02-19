namespace Sofco.Core.Services.Jobs
{
    public interface IHealthInsuranceJobService
    {
        void SyncHealthInsurance();

        void SyncPrepaidHealth();
    }
}