using System.Collections.Generic;
using Sofco.Domain.Crm.Billing;

namespace Sofco.Service.Crm.Interfaces
{
    public interface ICrmProjectService
    {
        List<CrmProject> GetAll();
    }
}