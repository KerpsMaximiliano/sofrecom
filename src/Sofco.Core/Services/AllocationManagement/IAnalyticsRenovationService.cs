using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IAnalyticsRenovationService
    {
        Response<List<AnalyticsRenovation>> GetAllByAnalyticId(int analyticId);
        Response<AnalyticsRenovation> Add(AnalyticsRenovation renovation);
        Response<AnalyticsRenovation> Update(AnalyticsRenovationModel renovationModel);
    }
}
