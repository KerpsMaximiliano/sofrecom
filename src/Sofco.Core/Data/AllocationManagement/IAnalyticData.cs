using Sofco.Core.Models.AllocationManagement;

namespace Sofco.Core.Data.AllocationManagement
{
    public interface IAnalyticData
    {
        AnalyticLiteModel GetLiteById(int analyticId);
    }
}