using System.Collections.Generic;
using Sofco.Core.Models;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IServicesService
    {
        Response<List<Service>> GetServices(string customerId);

        Response<List<SelectListModel>> GetServicesOptions(string customerId);

        Response<Service> GetService(string serviceId, string customerId);

        Analytic GetAnalyticByService(string serviceId);
        Response<List<SelectListModel>> GetAllNotRelatedOptions(string customerId);
        Response Update();
    }
}
