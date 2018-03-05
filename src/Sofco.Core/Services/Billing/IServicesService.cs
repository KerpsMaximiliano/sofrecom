using System.Collections.Generic;
using Sofco.Core.Models;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IServicesService
    {
        Response<List<CrmService>> GetServices(string customerId);

        Response<List<SelectListModel>> GetServicesOptions(string customerId);

        Response<CrmService> GetService(string serviceId, string customerId);

        bool HasAnalyticRelated(string serviceId);
    }
}
