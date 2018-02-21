using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using System;
using Sofco.Model.DTO;
using Sofco.Model.Models.AllocationManagement;

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
            context.Entry(employeeToChange).Property("Modified").IsModified = true;
            context.Entry(employeeToChange).Property("EndDate").IsModified = true;
        }

        public ICollection<Employee> Search(EmployeeSearchParams parameters)
        {
            IQueryable<Employee> query = context.Employees;

            if (!string.IsNullOrWhiteSpace(parameters.Name))
                query = query.Where(x => x.Name != null && x.Name.ToLowerInvariant().Contains(parameters.Name.ToLowerInvariant()));

            if (!string.IsNullOrWhiteSpace(parameters.Profile))
                query = query.Where(x => x.Profile != null && x.Profile.ToLowerInvariant().Contains(parameters.Profile.ToLowerInvariant()));

            if (!string.IsNullOrWhiteSpace(parameters.Seniority))
                query = query.Where(x => x.Seniority != null && x.Seniority.ToLowerInvariant().Contains(parameters.Seniority.ToLowerInvariant()));

            if (!string.IsNullOrWhiteSpace(parameters.Technology))
                query = query.Where(x => x.Technology != null && x.Technology.ToLowerInvariant().Contains(parameters.Technology.ToLowerInvariant()));

            if (parameters.Percentage.HasValue)
                query = query.Where(x => x.BillingPercentage == parameters.Percentage);

            return query.ToList();
        }

        public void Save(List<Employee> employees)
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
                } else
                {
                    Insert(item);
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
            return context.Employees.SingleOrDefault(x => x.Id == id);
        }
    }
}
