using System;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.TimeManagement;
using System.Collections.Generic;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IAllocationRepository : IBaseRepository<Allocation>
    {
        ICollection<Allocation> GetAllocationsForAnalyticDates(int employeeId, DateTime dateSince, DateTime dateTo);
        ICollection<Allocation> GetBetweenDaysByEmployeeId(int employeeId, DateTime startDate, DateTime endDate);
    }
}
