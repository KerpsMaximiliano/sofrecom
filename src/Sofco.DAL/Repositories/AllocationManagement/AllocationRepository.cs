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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;

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
                .Include(x => x.Analytic).ThenInclude(x => x.Sector)
                .Include(x => x.Employee)
                .OrderBy(x => x.AnalyticId).ThenBy(x => x.StartDate)
                .ToList();
        }

        public ICollection<Allocation> GetAllocationsBetweenDaysWithCharges(int employeeId, DateTime startDate, DateTime endDate)
        {
            return context.Allocations
                .Where(x => x.EmployeeId == employeeId && x.StartDate >= startDate && x.StartDate <= endDate)
                .Include(x => x.Analytic)
                .Include(x => x.Employee)
                .ThenInclude(x => x.SocialCharges)
                .OrderBy(x => x.AnalyticId).ThenBy(x => x.StartDate)
                .ToList();
        }

        public void AddReportPowerBi(List<ReportPowerBi> report)
        {
            context.ReportsPowerBi.AddRange(report);
        }

        public void CleanReportPowerBi()
        {
            context.Database.ExecuteSqlCommand("delete from app.reportsPowerBi");
        }

        public void UpdateModifiedUser(Allocation allocation)
        {
            context.Entry(allocation).Property("ModifiedBy").IsModified = true;
            context.Entry(allocation).Property("ModifiedAt").IsModified = true;
        }

        public void UpdateRealPercentage(Allocation allocation)
        {
            context.Entry(allocation).Property("RealPercentage").IsModified = true;
        }

        public IList<Allocation> GetAllocationsByDate(DateTime date)
        {
            return context.Allocations
                .Include(x => x.Analytic)
                .Include(x => x.Employee)
                .ThenInclude(x => x.SocialCharges)
                .Where(x => x.StartDate.Date == date.Date).ToList();
        }

        public IList<Allocation> GetAllocationsBetweenDay(DateTime date)
        {
            return context.Allocations
                .Include(x => x.Employee)
                .ThenInclude(x => x.SocialCharges)
                .Where(x => x.StartDate.Date >= date.Date && x.StartDate.Date <= date.Date)
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
                .Where(x => x.AnalyticId == analyticId && x.Employee.EndDate == null)
                .Select(x => x.Employee)
                .Include(x => x.EmployeeCategories)
                .Include(x => x.Allocations)
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

        public bool ExistCurrentAllocationByEmployeeAndManagerId(int employeeId, int managerId,
            int? analyticId, DateTime startDate, DateTime endDate)
        {
            if (analyticId.HasValue)
            {
                return context.Allocations
                    .Include(x => x.Analytic)
                    .Any(x => x.EmployeeId == employeeId
                              && x.Percentage > 0
                              && x.Analytic.ManagerId.GetValueOrDefault() == managerId
                              && x.AnalyticId == analyticId
                              && (x.StartDate.Date == startDate.Date || x.StartDate.Date == endDate.Date));
            }
            else
            {
                return context.Allocations
                    .Include(x => x.Analytic)
                    .Any(x => x.EmployeeId == employeeId
                              && x.Percentage > 0
                              && x.Analytic.ManagerId.GetValueOrDefault() == managerId
                              && (x.StartDate.Date == startDate.Date || x.StartDate.Date == endDate.Date));
            }
        }

        public bool ExistAllocationByEmployeeAndManagerId(int employeeId, int managerId, int? analyticId)
        {
            if (analyticId.HasValue)
            {
                return context.Allocations
                    .Include(x => x.Analytic)
                    .Any(x => x.EmployeeId == employeeId
                              && x.Percentage > 0
                              && x.Analytic.ManagerId.GetValueOrDefault() == managerId
                              && x.AnalyticId == analyticId);
            }
            else
            {
                return context.Allocations
                    .Include(x => x.Analytic)
                    .Any(x => x.EmployeeId == employeeId
                              && x.Percentage > 0
                              && x.Analytic.ManagerId.GetValueOrDefault() == managerId);
            }
        }

        public IList<Allocation> GetByEmployeesAndDate(List<int> employeeIds, int year, int month)
        {
            return context.Allocations.Include(x => x.Analytic).Where(x => employeeIds.Contains(x.EmployeeId) && x.StartDate.Date == new DateTime(year, month, 1).Date).ToList();
        }

        public ICollection<Allocation> GetAllocationsForWorkTimeReport(ReportParams parameters, DateTime startDate, DateTime endDate)
        {
            if (!parameters.AnalyticId.Any() && (!parameters.EmployeeId.HasValue || parameters.EmployeeId.Value == 0) && !parameters.ManagerId.Any())
            {
                return context.Allocations
                    .Include(x => x.Employee)
                        .ThenInclude(x => x.Manager)
                    .Include(x => x.Analytic)
                        .ThenInclude(x => x.Manager)
                    .Include(x => x.Analytic)
                        .ThenInclude(x => x.CostCenter)
                    .Include(x => x.Analytic)
                        .ThenInclude(x => x.Activity)
                    .Where(x => (!x.Employee.EndDate.HasValue || (x.Employee.EndDate.HasValue && x.Employee.EndDate.Value.Date > startDate.Date)) &&
                                !x.Employee.IsExternal &&
                                !x.Employee.ExcludeForTigerReport &&
                                (x.StartDate.Date == new DateTime(parameters.StartYear, parameters.StartMonth, 1).Date ||
                                 x.StartDate.Date == new DateTime(parameters.EndYear, parameters.EndMonth, 1).Date))
                    .ToList();
            }
            else
            {
                var query = context.Allocations
                     .Include(x => x.Analytic)
                     .Where(x => (!x.Employee.EndDate.HasValue || (x.Employee.EndDate.HasValue && x.Employee.EndDate.Value.Date > startDate.Date)) &&
                     !x.Employee.IsExternal &&
                     !x.Employee.ExcludeForTigerReport &&
                     (x.StartDate.Date == new DateTime(parameters.StartYear, parameters.StartMonth, 1).Date ||
                     x.StartDate.Date == new DateTime(parameters.EndYear, parameters.EndMonth, 1).Date));

                if (parameters.AnalyticId.Any())
                    query = query.Where(x => parameters.AnalyticId.Contains(x.AnalyticId));

                if (parameters.EmployeeId.HasValue && parameters.EmployeeId > 0)
                    query = query.Where(x => x.EmployeeId == parameters.EmployeeId.Value);

                if (parameters.ManagerId.Any())
                    query = query.Where(x => parameters.ManagerId.Contains(x.Analytic.ManagerId.GetValueOrDefault()));

                var allocations = query.ToList();

                var employeeIds = allocations.Select(x => x.EmployeeId).Distinct().ToList();

                return context.Allocations
                    .Include(x => x.Employee)
                    .ThenInclude(x => x.Manager)
                    .Include(x => x.Analytic)
                    .ThenInclude(x => x.Manager)
                    .Include(x => x.Analytic)
                    .ThenInclude(x => x.CostCenter)
                    .Include(x => x.Analytic)
                    .ThenInclude(x => x.Activity)
                    .Where(x => (employeeIds.Contains(x.EmployeeId) &&
                                (!x.Employee.EndDate.HasValue || (x.Employee.EndDate.HasValue && x.Employee.EndDate.Value.Date > startDate.Date))) &&
                                !x.Employee.IsExternal &&
                                !x.Employee.ExcludeForTigerReport &&
                                (x.StartDate.Date == new DateTime(parameters.StartYear, parameters.StartMonth, 1).Date ||
                                 x.StartDate.Date == new DateTime(parameters.EndYear, parameters.EndMonth, 1).Date))
                    .ToList();
            }
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
                            && ((x.StartDate.Month == startDate.Month && x.StartDate.Year == startDate.Year) 
                            || (x.StartDate.Month <= endDate.Month && x.StartDate.Year == endDate.Year)))
                .ToList();
        }

        public IList<Allocation> GetAllocationsLiteBetweenDatesAndFullBillingPercentage(DateTime date)
        {
            return context.Allocations
                  .Include(x => x.Analytic)
                  .Include(x => x.Employee)
                  .Where(x => (x.StartDate.Date <= date.Date
                               && x.ReleaseDate.Date >= date.Date) && x.Employee.BillingPercentage ==100).AsNoTracking() 
                  .ToList();
        }
    }
}
