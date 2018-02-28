using System.Collections.Generic;
using Sofco.Domain.Crm.Billing;

namespace Sofco.Core.Services.Billing
{
    public interface IServicesService
    {
        IList<CrmService> GetServices(string customerId);

        bool HasAnalyticRelated(string serviceId);
    }
}
