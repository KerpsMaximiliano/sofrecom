using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.ManagementReport;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.DAL.ManagementReport
{
    public interface IContractedDetailRepository : IBaseRepository<ContratedDetail>
    {
        List<ContratedDetail> GetByAnalytic(int IdAnalytic);
    }
}
