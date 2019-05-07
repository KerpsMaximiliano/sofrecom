using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.ManagementReport;
using System.Collections.Generic;

namespace Sofco.Core.DAL.ManagementReport
{
    public interface ICostDetailRepository : IBaseRepository<CostDetail>
    {
        List<CostDetail> GetByAnalytic(int IdAnalytic);
        List<CostDetailType> GetResourceTypes();
    }
}
