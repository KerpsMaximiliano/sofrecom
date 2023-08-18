using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.AllocationManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IAnalyticsRenovationRepository : IBaseRepository<AnalyticsRenovation>
    {
        List<AnalyticsRenovation> GetAllByAnalyticId(int analyticId);
        bool Exist(AnalyticsRenovation renovation);
    }
}
