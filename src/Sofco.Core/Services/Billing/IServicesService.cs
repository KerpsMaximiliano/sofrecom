using System.Collections.Generic;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IServicesService
    {
        IList<CrmService> GetServices(string customerId, string userMail, string userName);
    }
}
