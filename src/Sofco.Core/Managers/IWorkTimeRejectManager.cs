using Sofco.Domain.Utils;

namespace Sofco.Core.Managers
{
    public interface IWorkTimeRejectManager
    {
        Response Reject(int workTimeId, string comments);
    }
}