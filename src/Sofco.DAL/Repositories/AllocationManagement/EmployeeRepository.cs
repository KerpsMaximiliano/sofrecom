using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using System;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(SofcoContext context) : base(context)
        {
        }

        public bool Exist(int employeeId)
        {
            return context.Employees.Any(x => x.Id == employeeId);
        }

        public new ICollection<Employee> GetAll()
        {
            var now = DateTime.UtcNow.AddMonths(-4);

            return context.Employees.Include(x => x.Manager).Where(x => (x.EndDate == null || x.EndDate.Value.Date >= now.Date) && !x.IsExternal).ToList();
        }

        public ICollection<Employee> GetAllForWorkTimeReport()
        {
            var now = DateTime.UtcNow.AddMonths(-3);

            return context.Employees.Include(x => x.Manager).Where(x => (x.EndDate == null || (x.EndDate.HasValue && x.EndDate.Value.Date >= now.Date)) && !x.IsExternal).ToList();
        }

        public IList<Employee> GetByAnalyticIdInCurrentDate(int analyticId)
        {
            var date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            return context.Allocations.Include(x => x.Employee).Where(x =>
                x.AnalyticId == analyticId && x.Percentage > 0 && x.StartDate.Date == date.Date).Select(x => x.Employee).ToList();
        }

        public List<Employee> GetByEmployeeNumber(string[] employeeNumbers)
        {
            return context.Employees
                .Where(s => employeeNumbers.Contains(s.EmployeeNumber))
                .ToList();
        }

        public Employee GetByEmployeeNumber(string employeeNumber)
        {
            return context.Employees.SingleOrDefault(x => x.EmployeeNumber.Equals(employeeNumber));
        }

        public void UpdateEndDate(Employee employeeToChange)
        {
            context.Entry(employeeToChange).Property("CreatedByUser").IsModified = true;
            context.Entry(employeeToChange).Property("Modified").IsModified = true;
            context.Entry(employeeToChange).Property("EndDate").IsModified = true;
            context.Entry(employeeToChange).Property("EndReason").IsModified = true;
            context.Entry(employeeToChange).Property("TypeEndReasonId").IsModified = true;
        }

        public IList<Employee> GetUnassignedBetweenDays(DateTime startDate, DateTime endDate,
            string appSettingAnalyticBank)
        {
            var from = new DateTime(startDate.Year, startDate.Month, 1).Date;
            var to = new DateTime(endDate.Year, endDate.Month, 1).Date;

            var employeeIdsWithAllocations = context.Allocations
                .Include(x => x.Analytic)
                .Where(x => (x.StartDate.Date == from || x.StartDate.Date == to) && !x.Analytic.Title.Equals(appSettingAnalyticBank))
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();

            return context.Employees.Include(x => x.Manager).Where(x => !employeeIdsWithAllocations.Contains(x.Id) && x.EndDate == null && !x.IsExternal).ToList();
        }

        public IList<Employee> GetByAnalyticIds(List<int> analyticIds)
        {
            var ids = context.Allocations.Where(x => analyticIds.Contains(x.AnalyticId)
                                                     && x.Percentage > 0)
                .Select(x => x.Employee.Id)
                .Distinct()
                .ToList();

            return context.Employees
                .Include(x => x.WorkTimes)
                .Include(x => x.Licenses)
                .Where(x => ids.Contains(x.Id)).ToList();
        }

        public IList<Sector> GetAnalyticsWithSector(int employeeId)
        {
            var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date;

            return context.Allocations
                .Where(x => x.EmployeeId == employeeId && x.StartDate.Date == today)
                .Select(x => x.Analytic.Sector)
                .Include(x => x.ResponsableUser)
                .ToList();
        }

        public IList<Employee> GetByManagerId(int managerId)
        {
            return context.Employees
                .Where(x => x.ManagerId == managerId)
                .ToList();
        }

        public Employee GetByDocumentNumber(int dni)
        {
            return context.Employees.SingleOrDefault(x => x.DocumentNumber == dni);
        }

        public IList<Employee> GetMissingEmployess(IList<int> prepaidImportedDataIds)
        {
            return context.Employees.Where(x => !prepaidImportedDataIds.Contains(x.Id) && x.EndDate == null).ToList();
        }

        public IList<Tuple<int, string>> GetIdAndEmployeeNumber(int year, int month)
        {
            var date = new DateTime(year, month, 1).AddMonths(-2);

            return context.Employees
                .Where(x => x.EndDate == null || (x.EndDate.HasValue && x.EndDate.Value.Date >= date.Date))
                .Select(x => new Tuple<int, string>(x.Id, x.EmployeeNumber))
                .ToList();
        }

        public Employee GetWithSocialCharges(int employeeId)
        {
            return context.Employees.Include(x => x.Allocations).Include(x => x.SocialCharges).SingleOrDefault(x => x.Id == employeeId);
        }

        public ICollection<Employee> Search(EmployeeSearchParams parameters, DateTime startDate, DateTime endDate)
        {
            IQueryable<Employee> query = context.Employees.Include(x => x.Manager);

            if (parameters.Unassigned)
            {
                var employeeIdsWithAllocations = context.Allocations.Select(x => x.EmployeeId).Distinct().ToList();

                return query.Where(x => !employeeIdsWithAllocations.Contains(x.Id) && x.EndDate == null).ToList();
            }
            else
            {
                query = query.Where(x => x.EndDate == null);
            }

            query = query.Where(x => x.IsExternal == parameters.ExternalOnly);

            if (!string.IsNullOrWhiteSpace(parameters.Name))
                query = query.Where(x => x.Name != null && x.Name.ToLowerInvariant().Contains(parameters.Name.ToLowerInvariant()));

            if (!string.IsNullOrWhiteSpace(parameters.Profile))
                query = query.Where(x => x.Profile != null && x.Profile.ToLowerInvariant().Contains(parameters.Profile.ToLowerInvariant()));

            if (!string.IsNullOrWhiteSpace(parameters.Seniority))
                query = query.Where(x => x.Seniority != null && x.Seniority.ToLowerInvariant().Contains(parameters.Seniority.ToLowerInvariant()));

            if (!string.IsNullOrWhiteSpace(parameters.Technology))
                query = query.Where(x => x.Technology != null && x.Technology.ToLowerInvariant().Contains(parameters.Technology.ToLowerInvariant()));

            if (!string.IsNullOrWhiteSpace(parameters.EmployeeNumber))
                query = query.Where(x => x.EmployeeNumber != null && x.EmployeeNumber.ToLowerInvariant().Contains(parameters.EmployeeNumber.ToLowerInvariant()));

            if (parameters.SuperiorId.HasValue && parameters.SuperiorId.Value > 0)
                query = query.Where(x => x.ManagerId.GetValueOrDefault() == parameters.SuperiorId);

            if (parameters.Percentage.HasValue)
                query = query.Where(x => x.BillingPercentage == parameters.Percentage);

            if (parameters.AnalyticId.HasValue && parameters.AnalyticId > 0)
            {
                query = from employee in query
                        from allocation in context.Allocations
                        where employee.Id == allocation.EmployeeId && allocation.AnalyticId == parameters.AnalyticId &&
                              allocation.Percentage > 0 &&
                              (allocation.StartDate.Date == startDate.Date || allocation.StartDate.Date == endDate.Date)
                        select employee;
            }

            return query.Distinct().ToList();
        }

        public void ResetAllExamDays()
        {
            context.Database.ExecuteSqlCommand("UPDATE app.Employees SET ExamDaysTaken = 0");
        }

        public IList<EmployeeCategory> GetEmployeeCategories(int employeeId)
        {
            return context.EmployeeCategories
                .Include(x => x.Category)
                    .ThenInclude(x => x.Tasks)
                .Where(x => x.EmployeeId == employeeId && x.Category.Active).ToList();
        }

        public void UpdateBusinessHours(Employee employee)
        {
            context.Entry(employee).Property("BusinessHours").IsModified = true;
            context.Entry(employee).Property("BusinessHoursDescription").IsModified = true;
            context.Entry(employee).Property("OfficeAddress").IsModified = true;
            context.Entry(employee).Property("HolidaysPendingByLaw").IsModified = true;
            context.Entry(employee).Property("ManagerId").IsModified = true;
            context.Entry(employee).Property("HolidaysPending").IsModified = true;
            context.Entry(employee).Property("BillingPercentage").IsModified = true;
            context.Entry(employee).Property("HasCreditCard").IsModified = true;
            context.Entry(employee).Property("ExtraHolidaysQuantityByLaw").IsModified = true;
            context.Entry(employee).Property("ExtraHolidaysQuantity").IsModified = true;
            context.Entry(employee).Property("HasExtraHolidays").IsModified = true;
        }

        public IList<Employee> GetByEmployeeNumbers(IEnumerable<string> employeeNumbers)
        {
            return context.Employees.Where(x => employeeNumbers.Contains(x.EmployeeNumber)).ToList();
        }

        public IList<Employee> SearchUnemployees(UnemployeeSearchParameters parameters)
        {
            IQueryable<Employee> query = context.Employees.Include(x => x.TypeEndReason).Where(x => x.EndDate.HasValue);

            if (!string.IsNullOrWhiteSpace(parameters.Name))
                query = query.Where(x => x.Name != null && x.Name.ToLowerInvariant().Contains(parameters.Name.ToLowerInvariant()));

            if (parameters.StartDate.HasValue)
                query = query.Where(x => x.EndDate.HasValue && x.EndDate.Value.Date >= parameters.StartDate.Value.Date);

            if (parameters.EndDate.HasValue)
                query = query.Where(x => x.EndDate.HasValue && x.EndDate.Value.Date <= parameters.EndDate.Value.Date);

            return query.ToList();
        }

        public void Save(List<Employee> employees)
        {
            var storedItems = GetByEmployeeNumber(employees.Select(s => s.EmployeeNumber).ToArray());

            var storedNumbers = storedItems.Select(s => s.EmployeeNumber).ToList();

            foreach (var item in employees)
            {
                if (storedNumbers.Contains(item.EmployeeNumber))
                {
                    var updateItem = storedItems
                        .First(s => s.EmployeeNumber == item.EmployeeNumber);

                    Update(updateItem, item);
                }
                else
                {
                    Insert(item);
                }
            }

            context.SaveChanges();
        }

        public void Save(Employee employee)
        {
            var storedItem = GetByEmployeeNumber(employee.EmployeeNumber);
            if (storedItem == null)
            {
                Insert(employee);
                return;
            }

            Update(storedItem, employee);

            context.SaveChanges();
        }

        public IList<Employee> GetByAnalyticWithWorkTimes(int analyticId)
        {
            var ids = context.Allocations.Where(x => x.AnalyticId == analyticId && x.Percentage > 0)
                .Select(x => x.Employee.Id)
                .Distinct()
                .ToList();

            return context.Employees
                .Include(x => x.WorkTimes)
                .Include(x => x.Licenses)
                .Include(x => x.Allocations)
                .Where(x => ids.Contains(x.Id)).ToList();
        }

        public void Update(List<Employee> employees)
        {
            var storedItems = GetByEmployeeNumber(employees.Select(s => s.EmployeeNumber).ToArray());

            var storedNumbers = storedItems.Select(s => s.EmployeeNumber).ToList();

            foreach(var item in employees)
            {
                if (!storedNumbers.Contains(item.EmployeeNumber)) continue;

                var updateItem = storedItems.First(s => s.EmployeeNumber == item.EmployeeNumber);

                Update(updateItem, item);
            }

            context.SaveChanges();
        }

        private void Update(Employee storedData, Employee data)
        {
            storedData.Birthday = data.Birthday;
            storedData.StartDate = data.StartDate;
            storedData.EndDate = data.EndDate;
            storedData.Name = data.Name;
            storedData.Profile = data.Profile;
            storedData.Technology = data.Technology;
            storedData.Seniority = data.Seniority;
            storedData.Address = data.Address;
            storedData.Location = data.Location;
            storedData.Province = data.Province;
            storedData.Country = data.Country;
            storedData.HealthInsuranceCode = data.HealthInsuranceCode;
            storedData.PrepaidHealthCode = data.PrepaidHealthCode;
            storedData.OfficeAddress = data.OfficeAddress;
            storedData.Email = data.Email;
            storedData.Bank = data.Bank;

            Update(storedData);
        }

        public List<Employee> GetByEndDate(DateTime date)
        {
            return context.Employees
                .Where(s =>
                s.EndDate != null
                && s.EndDate.Value.Date >= date)
                .ToList();
        }

        public Employee GetById(int id)
        {
            return context.Employees.Include(x => x.Manager).SingleOrDefault(x => x.Id == id);
        }

        public List<Employee> GetById(int[] ids)
        {
            return context.Employees
                .Where(s => ids.Contains(s.Id))
                .ToList();
        }

        public Employee GetByEmail(string email)
        {
            return context.Employees.Include(x => x.Manager).SingleOrDefault(x => x.Email == email);
        }

        public Employee GetByEmailWithDiscounts(string email)
        {
            return context.Employees.Include(x => x.SalaryDiscounts).SingleOrDefault(x => x.Email == email);
        }

        public IList<Employee> GetByAnalyticWithSocialCharges(int idAnalytic, DateTime startDate, DateTime endDate)
        {
            var ids = context.Allocations.Where(x => x.AnalyticId == idAnalytic && x.Percentage > 0 && x.StartDate.Date >= startDate.Date && x.StartDate.Date <= endDate.Date)
                .Select(x => x.Employee.Id)
                .Distinct()
                .ToList();

            return context.Employees
                .Include(x => x.SocialCharges)
                .Include(x => x.Allocations)
                .Where(x => ids.Contains(x.Id)).ToList();
        }

        public void UpdateAssignComments(Employee employee)
        {
            context.Entry(employee).Property("AssignComments").IsModified = true;
        }

        public IList<Employee> GetUnassignedBetweenDays(DateTime startDate, DateTime endDate, int employeeId)
        {
            var from = new DateTime(startDate.Year, startDate.Month, 1).Date;
            var to = new DateTime(endDate.Year, endDate.Month, 1).Date;

            if (employeeId > 0)
            {
                return context.Employees.Include(x => x.Manager).Where(x => x.Id == employeeId && x.EndDate == null && !x.IsExternal).ToList();
            }
            else
            {
                var filter = context.Allocations.Where(x => x.StartDate.Date == from).Select(x => x.EmployeeId).Distinct().ToList();

                filter.AddRange(context.Allocations.Where(x => x.StartDate.Date == to).Select(x => x.EmployeeId).Distinct().ToList());

                filter = filter.Distinct().ToList();

                return context.Employees.Include(x => x.Manager).Where(x => !filter.Contains(x.Id) && x.EndDate == null && !x.IsExternal).ToList();
            }
        }

        public Employee GetUserInfo(string email)
        {
            return context.Employees.Include(x => x.Manager)
                .Include(x => x.Allocations)
                    .ThenInclude(x => x.Analytic)
                        .ThenInclude(x => x.Sector)
                .SingleOrDefault(x => x.Email == email);
        }
    }
}
