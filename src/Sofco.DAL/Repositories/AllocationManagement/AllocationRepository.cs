using System;
using System.Collections.Generic;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class AllocationRepository : BaseRepository<Allocation>, IAllocationRepository
    {
        public AllocationRepository(SofcoContext context) : base(context)
        {
        }

        public ICollection<Allocation> GetAllocationsBetweenDays(int employeeId, DateTime startDate, DateTime endDate)
        {
            return context.Allocations
                .Where(x => x.EmployeeId == employeeId && ((x.StartDate >= startDate && x.StartDate <= endDate)))
                .Include(x => x.Analytic)
                .Include(x => x.Employee)
                .OrderBy(x => x.AnalyticId).ThenBy(x => x.StartDate)
                .ToList();
        }

        public void UpdateReleaseDate(Allocation allocation)
        {
            context.Entry(allocation).Property("ReleaseDate").IsModified = true;
        }

        public void UpdatePercentage(Allocation allocation)
        {
            context.Entry(allocation).Property("Percentage").IsModified = true;
        }
    }
}
