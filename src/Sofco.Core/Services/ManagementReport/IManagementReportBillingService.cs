using Sofco.Core.Models.ManagementReport;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.ManagementReport
{
    public interface IManagementReportBillingService
    {
        Response Update(UpdateValueModel model);

        Response UpdateData(UpdateBillingDataModel model);
    }
}
