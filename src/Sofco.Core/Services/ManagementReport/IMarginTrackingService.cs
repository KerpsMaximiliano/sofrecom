using System.Collections.Generic;
using Sofco.Core.Models.ManagementReport;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.ManagementReport
{
    public interface IMarginTrackingService
    {
        Response<IList<MarginTrackingModel>> Get(int managementReportId);
    }
}
