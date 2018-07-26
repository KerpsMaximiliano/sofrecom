using System.Collections.Generic;
using Sofco.Core.Models;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IServicesService
    {
        Response<List<Service>> GetServices(string customerId);

        Response<List<SelectListModel>> GetServicesOptions(string customerId);

        Response<Service> GetService(string serviceId, string customerId);

        Analytic GetAnalyticByService(string serviceId);
    }
}
