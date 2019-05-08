using Sofco.Domain.Utils;

namespace Sofco.Core.Services.ManagementReport
{
    public interface IManagementReportBillingService
    {
        Response Update(int id, decimal value);
    }
}
