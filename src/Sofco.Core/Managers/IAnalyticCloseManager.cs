using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Core.Managers
{
    public interface IAnalyticCloseManager
    {
        Response Close(int analyticId, AnalyticStatus status);
    }
}