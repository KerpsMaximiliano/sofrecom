using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using System;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Model.DTO;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Relationships;

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
            return context.Employees.Where(x => x.EndDate == null).ToList();
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
         
        public ICollection<Employee> Search(EmployeeSearchParams parameters)
        {
            IQueryable<Employee> query = context.Employees;

            if (parameters.Unassigned)
            {
                var employeeIdsWithAllocations = context.Allocations.Select(x => x.EmployeeId).Distinct().ToList();

                return query.Where(x => !employeeIdsWithAllocations.Contains(x.Id)).ToList();
            }

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

            if (parameters.Percentage.HasValue)
                query = query.Where(x => x.BillingPercentage == parameters.Percentage);

            if (parameters.AnalyticId.HasValue && parameters.AnalyticId > 0)
            {
                query = from employee in query
                        from allocation in context.Allocations
                        where employee.Id == allocation.EmployeeId && allocation.AnalyticId == parameters.AnalyticId
                        select employee;
            }

            return query.Distinct().ToList();
        }

        public void ResetAllExamDays()
        {
            context.Database.ExecuteSqlCommand("UPDATE app.Employees SET ExamDaysTaken = 0");
        }

        public void UpdateHolidaysPending(Employee employeeToModif)
        {
            context.Entry(employeeToModif).Property("HolidaysPending").IsModified = true;
        }

        public void UpdateExamDaysTaken(Employee employeeToModif)
        {
            context.Entry(employeeToModif).Property("ExamDaysTaken").IsModified = true;
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

        public void Update(List<Employee> employees)
        {
            var storedItems = GetByEmployeeNumber(employees.Select(s => s.EmployeeNumber).ToArray());

            var storedNumbers = storedItems.Select(s => s.EmployeeNumber).ToList();

            foreach(var item in employees)
            {
                if(storedNumbers.Contains(item.EmployeeNumber))
                {
                    var updateItem = storedItems
                        .First(s => s.EmployeeNumber == item.EmployeeNumber);

                    Update(updateItem, item);
                }
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

        public Employee GetByEmail(string email)
        {
            return context.Employees.SingleOrDefault(x => x.Email == email);
        }
    }
}
