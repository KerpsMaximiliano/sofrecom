using System.Collections.Generic;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Core.Managers
{
    public interface IWorkTimeSendMailManager
    {
        void SendEmail(List<WorkTime> workTimes);
    }
}