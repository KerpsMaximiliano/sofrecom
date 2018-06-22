using System.Collections.Generic;
using Sofco.Core.Models;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IServicesService
    {
        Response<List<CrmService>> GetServices(string customerId, bool getAll);

        Response<List<SelectListModel>> GetServicesOptions(string customerId, bool getAll);

        Response<CrmService> GetService(string serviceId, string customerId);

        Analytic GetAnalyticByService(string serviceId);
    }
}
