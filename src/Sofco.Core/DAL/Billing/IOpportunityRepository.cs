using System;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.DAL.Billing
{
    public interface IOpportunityRepository : IBaseRepository<Opportunity>
    {
        Opportunity GetByCrmId(string crmOpportunityId);
    }
}
