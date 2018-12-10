using System.Collections.Generic;
using Sofco.Domain.Crm.Billing;

namespace Sofco.Service.Crm.Interfaces
{
    public interface ICrmServiceService
    {
        List<CrmService> GetAll();
    }
}