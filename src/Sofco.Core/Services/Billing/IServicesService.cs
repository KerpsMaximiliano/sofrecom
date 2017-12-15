using System.Collections.Generic;
using Sofco.Domain.Crm.Billing;

namespace Sofco.Core.Services.Billing
{
    public interface IServicesService
    {
        IList<CrmService> GetServices(string customerId, string userMail, string userName);
    }
}
