using System.Collections.Generic;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.Data.AllocationManagement
{
    public interface IAnalyticData
    {
        AnalyticLiteModel GetLiteById(int analyticId);

        List<Analytic> GetByManagerId(int managerId);
    }
}