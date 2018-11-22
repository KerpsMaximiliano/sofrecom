using System.Collections.Generic;
using Sofco.Domain.Crm;

namespace Sofco.Service.Crm.Interfaces
{
    public interface ICrmAccountService
    {
        List<CrmAccount> GetAll();
    }
}