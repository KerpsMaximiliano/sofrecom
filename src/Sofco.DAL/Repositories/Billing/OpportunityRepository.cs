using System.Linq;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Billing;

namespace Sofco.DAL.Repositories.Billing
{
    public class OpportunityRepository : BaseRepository<Opportunity>, IOpportunityRepository
    {
        public OpportunityRepository(SofcoContext context) : base(context)
        {
        }

        public Opportunity GetByCrmId(string crmOpportunityId)
        {
            return context.Opportunities.SingleOrDefault(x => x.CrmId == crmOpportunityId);
        }
    }
}
