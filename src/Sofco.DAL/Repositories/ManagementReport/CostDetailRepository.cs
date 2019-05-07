using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.ManagementReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.DAL.Repositories.ManagementReport
{
    public class CostDetailRepository : BaseRepository<CostDetail>, ICostDetailRepository
    {
        public CostDetailRepository(SofcoContext context) : base(context)
        {
        }

        public List<CostDetail> GetByAnalytic(int IdAnalytic)
        {
            var CostDetail = context.CostDetails
                                        .Where(x => x.IdAnalytic == IdAnalytic)
                                        .Include(x => x.Type)
                                        .Include(e => e.Employee)
                                        .ToList();
            return CostDetail;
        }

        public List<CostDetailType> GetResourceTypes()
        {
            return context.CostDetailTypes.OrderBy(t => t.Id).ToList();
        }


    }
}
