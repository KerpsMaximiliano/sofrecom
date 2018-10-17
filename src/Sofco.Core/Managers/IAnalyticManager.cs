using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Managers
{
    public interface IAnalyticManager
    {
        Response UpdateCrmAnalytic(Analytic analytic);
    }
}