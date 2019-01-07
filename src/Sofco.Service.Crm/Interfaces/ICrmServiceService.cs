using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Domain.Crm;

namespace Sofco.Service.Crm.Interfaces
{
    public interface ICrmServiceService
    {
        List<CrmService> GetAll();

        Result ActivateService(Guid serviceId, bool activate = true);

        Result DeactivateService(Guid serviceId);

        Result Update(CrmServiceUpdate crmServiceUpdate);
    }
}