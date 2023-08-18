using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.AllocationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class AnalyticsRenovationRepository : BaseRepository<AnalyticsRenovation>, IAnalyticsRenovationRepository
    {
        public AnalyticsRenovationRepository(SofcoContext context) : base(context)
        {
            
        }

        public List<AnalyticsRenovation> GetAllByAnalyticId(int analyticId)
        {
            var renovations = context.AnalyticsRenovations
                                .Include(x => x.Analytic)
                                .Include(x => x.Opportunity)
                                .Where(x => x.AnalyticId == analyticId)
                                .ToList();
            return renovations;
        }

        public bool Exist(AnalyticsRenovation renovation)
        {
            return context.AnalyticsRenovations.Any(x => x.Orden == renovation.Orden 
                                                      && x.Renovation == renovation.Renovation
                                                      && x.OpportunityId == renovation.OpportunityId);
        }
    }
}
