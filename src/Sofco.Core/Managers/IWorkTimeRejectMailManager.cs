using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Core.Managers
{
    public interface IWorkTimeRejectMailManager
    {
        void SendEmail(WorkTime workTime);
    }
}