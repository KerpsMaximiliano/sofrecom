using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.ManagementReport;
using System.Collections.Generic;

namespace Sofco.Core.DAL.ManagementReport
{
    public interface ICostDetailRepository : IBaseRepository<CostDetail>
    {
        List<CostDetailType> GetResourceTypes();
    }
}
