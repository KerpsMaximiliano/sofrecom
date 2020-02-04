using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.ManagementReport;

namespace Sofco.Core.DAL.ManagementReport
{
    public interface ICostDetailResourceRepository : IBaseRepository<CostDetailResource>
    {
        IList<CostDetailResource> GetByAnalyticAndDates(int managementId, DateTime firstMonthDate, DateTime lastMonthDate);
        IList<CostDetailResource> GetByDate(DateTime date);
        int GetIdIfExist(int costDetailId, int resourceEmployeeId, int auxBudgetTypeId);
    }
}
