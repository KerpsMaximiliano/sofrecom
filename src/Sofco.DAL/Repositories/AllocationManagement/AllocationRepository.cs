using System;
using System.Collections.Generic;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;

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
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var employees = context.Allocations
                .Include(x => x.Analytic)
                .Where(x => x.Analytic.ServiceId.Equals(serviceId) 
                            && x.StartDate.Date == date.Date
                            && x.Percentage > 0)
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();

            return context.Employees.Include(x => x.EmployeeCategories).Where(x => employees.Contains(x.Id)).ToList();
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

        public IList<Allocation> GetLastAllocationsForEmployee(int employeeId, DateTime date)
        {
            return context.Allocations
                .Where(x => x.EmployeeId == employeeId
                            && x.StartDate.Date > date.Date
                            && x.Percentage > 0)
                .ToList();
        }

        public DateTime GetStartDate(int analitycId, int employeeId)
        {
            return context.Allocations.Where(x => x.AnalyticId == analitycId && x.EmployeeId == employeeId && x.Percentage > 0).Min(x => x.StartDate);
        }

        public int GetResourceQuantityByDate(int analyticId, DateTime dateTime)
        {
            return context.Allocations.Count(x => x.AnalyticId == analyticId && x.Percentage > 0 && x.StartDate.Date == dateTime);
        }

        public void Clean()
        {
            context.Database.ExecuteSqlCommand("delete from app.Allocations where Percentage = 0");
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
                .Include(x => x.Analytic)
                .Where(x => x.EmployeeId == employeeId
                            && x.Percentage > 0
                            && (x.StartDate.Date == startDate.Date
                            || x.StartDate.Date == endDate.Date))
                .ToList();
        }

        public bool ExistCurrentAllocationByEmployeeAndManagerId(int employeeId, int managerId, DateTime startDate)
        {
            return context.Allocations
                .Include(x => x.Analytic)
                .Any(x => x.EmployeeId == employeeId
                          && x.Percentage > 0
                          && x.Analytic.ManagerId.GetValueOrDefault() == managerId
                          && x.StartDate.Date == startDate.Date);
        }

        public ICollection<Allocation> GetAllocationsForWorkTimeReport(ReportParams parameters)
        {
            var query = context.Allocations
                .Include(x => x.Employee)
                .Include(x => x.Analytic)
                    .ThenInclude(x => x.Manager)
                .Include(x => x.Analytic)
                    .ThenInclude(x => x.CostCenter)
                .Include(x => x.Analytic)
                    .ThenInclude(x => x.Activity)
                .Where(x => !x.Employee.EndDate.HasValue && 
                            (x.StartDate.Date == new DateTime(parameters.StartYear, parameters.StartMonth, 1).Date ||
                            x.StartDate.Date == new DateTime(parameters.EndYear, parameters.EndMonth, 1).Date));

            //if (parameters.AnalyticId.HasValue && parameters.AnalyticId > 0)
            //    query = query.Where(x => x.AnalyticId == parameters.AnalyticId.Value);

            if (parameters.EmployeeId.HasValue && parameters.EmployeeId > 0)
                query = query.Where(x => x.EmployeeId == parameters.EmployeeId.Value);

            if (parameters.ManagerId.HasValue && parameters.ManagerId > 0)
                query = query.Where(x => x.Analytic.ManagerId.GetValueOrDefault() == parameters.ManagerId.Value);

            return query.ToList();
        }

        public bool Exist(int allocationId)
        {
            return context.Allocations.Any(x => x.Id == allocationId);
        }

        public ICollection<Employee> GetByEmployeesForReport(AllocationReportParams parameters)
        {
            IQueryable<Allocation> query = context.Allocations
                .Include(x => x.Analytic)
                .Include(x => x.Employee)
                    .ThenInclude(x => x.Manager)
                .Where(x => x.StartDate.Date >= parameters.StartDate.GetValueOrDefault().Date && x.StartDate.Date <= parameters.EndDate.GetValueOrDefault().Date && !x.Employee.EndDate.HasValue);

            if (!parameters.IncludeStaff)
                query = query.Where(x => x.Employee.BillingPercentage != 0);

            if (!parameters.Unassigned)
            {
                if (parameters.AnalyticIds.Any())
                    query = query.Where(x => parameters.AnalyticIds.Contains(x.AnalyticId));

                if (parameters.EmployeeId.HasValue)
                    query = query.Where(x => x.EmployeeId == parameters.EmployeeId.Value);

                if (parameters.IncludeAnalyticId == 2)
                    query = query.Where(x => x.Analytic.Status == AnalyticStatus.Open);

                if (parameters.IncludeAnalyticId == 3)
                    query = query.Where(x => x.Analytic.Status == AnalyticStatus.Close || x.Analytic.Status == AnalyticStatus.CloseToExpenses);
            }

            return query.Select(x => x.Employee)
                .Include(x => x.Manager)
                .Include(x => x.Allocations)
                .ThenInclude(x => x.Analytic)
                .OrderBy(x => x.StartDate)
                .Distinct()
                .ToList();
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

        public ICollection<Allocation> GetAllocationsLiteBetweenDaysForWorkTimeControl(int employeeId, DateTime startDate, DateTime endDate)
        {
            return context.Allocations
                .Where(x => x.EmployeeId == employeeId
                            && x.Percentage > 0 
                            && x.StartDate.Date >= startDate.Date 
                            && x.StartDate.Date <= endDate.Date)
                .ToList();
        }
    }
}
