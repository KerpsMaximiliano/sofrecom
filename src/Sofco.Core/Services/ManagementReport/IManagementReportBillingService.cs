using Sofco.Core.Models.ManagementReport;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.ManagementReport
{
    public interface IManagementReportBillingService
    {
        Response<int> Update(UpdateValueModel model);

        Response<int> UpdateData(UpdateBillingDataModel model);
    }
}
