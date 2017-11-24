using System.Collections.Generic;
using Sofco.Domain.Crm.Billing;

namespace Sofco.Core.Data.Billing
{
    public interface IServiceData
    {
        IList<CrmService> GetServices(string customerId, string identityName, string userMail, bool hasDirectorGroup);
    }
}
