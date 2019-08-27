using System.Collections.Generic;
using Sofco.Core.Models.ManagementReport;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.ManagementReport
{
    public interface IManagementReportBillingService
    {
        Response<int> Update(UpdateValueModel model);

        Response<int> UpdateData(UpdateBillingDataModel model);

        Response UpdateQuantityResources(int idBilling, int quantityResources);

        Response AddResources(int idBilling, IList<ResourceBillingRequest> resources);
    }
}
