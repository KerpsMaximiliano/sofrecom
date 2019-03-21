using Sofco.Core.Models.ManagementReport;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.ManagementReport
{
    public interface IManagementReportService
    {
        Response<ManagementReportDetail> GetDetail(string serviceId);
        Response<BillingDetail> GetBilling(string serviceId);
    }
}
