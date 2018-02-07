using System;
using System.Collections.Generic;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Model.DTO;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

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
                .Where(x => x.EmployeeId == employeeId && x.StartDate >= startDate && x.StartDate <= endDate)
                .Include(x => x.Analytic)
                .Include(x => x.Employee)
                .OrderBy(x => x.AnalyticId).ThenBy(x => x.StartDate)
                .ToList();
        }

        public void UpdateReleaseDate(Allocation allocation)
        {
            context.Entry(allocation).Property("ReleaseDate").IsModified = true;
        }

        public ICollection<Employee> GetByService(string serviceId)
        {
            return context.Allocations
                .Include(x => x.Analytic)
                .Include(x => x.Employee)
                .Where(x => x.Analytic.ServiceId.Equals(serviceId))
                .Select(x => x.Employee)
                .Distinct()
                .ToList();
        }

        public ICollection<Allocation> GetByEmployee(int id)
        {
            return context.Allocations
                .Include(x => x.Analytic)
                .Where(x => x.EmployeeId == id)
                .ToList();
        }

        public ICollection<Employee> GetByEmployeesForReport(AllocationReportParams parameters)
        {
            IQueryable<Allocation> query = context.Allocations
                .Include(x => x.Employee)
                .Where(x => x.StartDate >= parameters.StartDate && x.StartDate <= parameters.EndDate);

            if (parameters.AnalyticId.HasValue)
                query = query.Where(x => x.AnalyticId == parameters.AnalyticId.Value);

            if (parameters.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == parameters.EmployeeId.Value);

            if (!parameters.IncludeStaff)
                query = query.Where(x => x.Employee.BillingPercentage != 0);

            if (parameters.Percentage.HasValue)
            {
                if (parameters.Percentage == 999)
                    query = query.Where(x => x.Percentage != 100);
                else
                    query = query.Where(x => x.Percentage == parameters.Percentage);
            }

            return query.Select(x => x.Employee).Distinct().ToList();
        }

        public IList<decimal> GetAllPercentages()
        {
            return context.Allocations.Select(x => x.Percentage).Distinct().ToList();
        }

        public void UpdatePercentage(Allocation allocation)
        {
            context.Entry(allocation).Property("Percentage").IsModified = true;
        }
    }
}
