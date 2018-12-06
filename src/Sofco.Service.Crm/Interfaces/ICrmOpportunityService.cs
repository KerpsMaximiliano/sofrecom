using System.Collections.Generic;
using Sofco.Domain.Crm;

namespace Sofco.Service.Crm.Interfaces
{
    public interface ICrmOpportunityService
    {
        List<CrmOpportunity> GetAll();
    }
}