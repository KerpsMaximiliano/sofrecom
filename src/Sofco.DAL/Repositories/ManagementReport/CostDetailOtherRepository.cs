using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.ManagementReport;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.DAL.Repositories.ManagementReport
{
    public class CostDetailOtherRepository : BaseRepository<CostDetailOther>, ICostDetailOtherRepository
    {
        public CostDetailOtherRepository(SofcoContext context) : base(context)
        {
        }

        public List<CostDetailOther> GetByTypeAndCostDetail(int costTipeId, int costDetailId)
        {
            return context.CostDetailOthers
                        //.Include(x => x.CostDetailType)
                        .Include(x => x.CostDetailSubtype)
                            .ThenInclude(y => y.CostDetailType)
                        .Where(x => x.CostDetailSubtype.CostDetailTypeId == costTipeId
                                && x.CostDetailId == costDetailId)
                        .ToList();
        }
    }
}
