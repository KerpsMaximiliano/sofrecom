namespace Sofco.WebJob.Jobs.Interfaces
{
    public interface IEmployeeEndJob : IJob
    {
        void SendNotification();
    }
}
