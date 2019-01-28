using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Managers
{
    public interface IWorkTimeRejectManager
    {
        Response Reject(int workTimeId, string comments, bool massive);

        void SendGeneralRejectMail(WorkTime workTime);
    }
}