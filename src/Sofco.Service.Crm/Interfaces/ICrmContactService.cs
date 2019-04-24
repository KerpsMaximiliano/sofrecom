using System.Collections.Generic;
using Sofco.Domain.Crm;

namespace Sofco.Service.Crm.Interfaces
{
    public interface ICrmContactService
    {
        List<CrmContact> GetAll();
    }
}
