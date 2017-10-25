using System;
using System.Collections.Generic;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.TimeManagement;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class AllocationRepository : BaseRepository<Allocation>, IAllocationRepository
    {
        public AllocationRepository(SofcoContext context) : base(context)
        {
        }

        public ICollection<Allocation> GetAllocationsForAnalyticDates(int employeeId, DateTime dateSince, DateTime dateTo)
        {
            return context.Allocations
                .Where(x => x.EmployeeId == employeeId && ((x.StartDate >= dateSince && x.StartDate <= dateTo) || (x.EndDate >= dateSince && x.EndDate <= dateTo)))
                .Include(x => x.Analytic)
                .Include(x => x.Employee)
                .OrderBy(x => x.StartDate).ThenBy(x => x.EndDate)
                .ToList();
        }

        public ICollection<Allocation> GetBetweenDaysByEmployeeId(int employeeId, DateTime startDate, DateTime endDate)
        {
            return context.Allocations
                .Where(x => x.EmployeeId == employeeId && ((startDate >= x.StartDate && startDate <= x.EndDate) || (endDate >= x.StartDate && endDate <= x.EndDate)))
                .Include(x => x.Analytic)
                .Include(x => x.Employee)
                .OrderBy(x => x.StartDate).ThenBy(x => x.EndDate)
                .ToList();
        }
    }
}
