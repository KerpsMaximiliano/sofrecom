using System;
using System.Collections.Generic;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

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

        public ICollection<Employee> GetByAnalyticId(int analyticId)
        {
            return context.Allocations
                .Include(x => x.Analytic)
                .Include(x => x.Employee)
                .Where(x => x.AnalyticId == analyticId)
                .Select(x => x.Employee)
                .Distinct()
                .ToList();
        }

        public void DeleteAllocationWithReleaseDateNull()
        {
            context.Database.ExecuteSqlCommand("delete from app.allocations where releasedate = '0001-01-01 00:00:00.0000000'");
        }

        public IList<Allocation> GetLastAllocationsForEmployee(int id, DateTime now)
        {
            return context.Allocations.Where(x => x.EmployeeId == id && x.StartDate.Date > now.Date).ToList();
        }

        public IList<User> GetManagers(int employeeId, DateTime dateFrom, DateTime dateTo)
        {
            var analyticIds = context.Allocations
                .Where(x => x.EmployeeId == employeeId && (x.StartDate.Date == dateFrom.Date || x.StartDate.Date == dateTo.Date))
                .Select(x => x.AnalyticId)
                .Distinct()
                .ToList();

            var users = context.Analytics
                .Include(x => x.Manager)
                .Where(x => analyticIds.Contains(x.Id))
                .Select(x => x.Manager)
                .Distinct()
                .ToList();

            return users;
        }

        public ICollection<Allocation> GetByEmployee(int id)
        {
            return context.Allocations
                .Include(x => x.Analytic)
                .Where(x => x.EmployeeId == id)
                .ToList();
        }

        public ICollection<Allocation> GetAllocationsLiteBetweenDays(int employeeId, DateTime startDate, DateTime endDate)
        {
            return context.Allocations
                .Where(x => x.EmployeeId == employeeId && x.StartDate >= startDate && x.StartDate <= endDate)
                .ToList();
        }

        public ICollection<Allocation> GetAllocationsForWorktimeReport(ReportParams parameters)
        {
            IQueryable<Allocation> query = context.Allocations
                .Include(x => x.Employee)
                .Include(x => x.Analytic)
                    .ThenInclude(x => x.Manager)
                .Where(x => x.StartDate.Date == new DateTime(parameters.Year, parameters.Month, 1).Date ||
                            x.StartDate.Date == new DateTime(parameters.Year, parameters.Month-1, 1).Date);

            if (parameters.AnalyticId.HasValue && parameters.AnalyticId > 0)
                query = query.Where(x => x.AnalyticId == parameters.AnalyticId.Value);

            if (parameters.EmployeeId.HasValue && parameters.EmployeeId > 0)
                query = query.Where(x => x.EmployeeId == parameters.EmployeeId.Value);

            if (parameters.ManagerId.HasValue && parameters.ManagerId > 0)
                query = query.Where(x => x.Analytic.ManagerId.GetValueOrDefault() == parameters.ManagerId.Value);

            if (!string.IsNullOrWhiteSpace(parameters.ClientId) && !parameters.ClientId.Equals("0"))
                query = query.Where(x => x.Analytic.ClientExternalId == parameters.ClientId);

            return query.ToList();
        }

        public bool Exist(int allocationId)
        {
            return context.Allocations.Any(x => x.Id == allocationId);
        }

        public ICollection<Employee> GetByEmployeesForReport(AllocationReportParams parameters)
        {
            IQueryable<Allocation> query = context.Allocations
                .Include(x => x.Employee)
                .Where(x => x.StartDate >= parameters.StartDate && x.StartDate <= parameters.EndDate);

            if (parameters.AnalyticIds.Any())
                query = query.Where(x => parameters.AnalyticIds.Contains(x.AnalyticId));

            if (parameters.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == parameters.EmployeeId.Value);

            if (!parameters.IncludeStaff)
                query = query.Where(x => x.Employee.BillingPercentage != 0);

            //if (parameters.Percentage.HasValue)
            //{
            //    if (parameters.Percentage == 999)
            //        query = query.Where(x => x.Percentage != 100);
            //    else
            //        query = query.Where(x => x.Percentage == parameters.Percentage);
            //}

            return query.Select(x => x.Employee).Distinct().ToList();
        }

        public void RemoveAllocationByAnalytic(int analyticId, DateTime today)
        {
            var allocations = context.Allocations.Where(x => x.AnalyticId == analyticId && x.StartDate.Date > today.Date);
            context.Allocations.RemoveRange(allocations);
        }

        public void UpdatePercentage(Allocation allocation)
        {
            context.Entry(allocation).Property("Percentage").IsModified = true;
        }
    }
}
