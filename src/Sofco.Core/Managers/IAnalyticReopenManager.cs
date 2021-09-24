using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Core.Managers
{
    public interface IAnalyticReopenManager
    {
        Response Reopen(int analyticId, AnalyticStatus status);
    }
}
