using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.ManagementReport;
using System.Collections.Generic;

namespace Sofco.Core.DAL.ManagementReport
{
    public interface ICostDetailOtherRepository : IBaseRepository<CostDetailOther>
    {
        List<CostDetailOther> GetByTypeAndCostDetail(int costCategoryId, int costDetailId);
    }
}
