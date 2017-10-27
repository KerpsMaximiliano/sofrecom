using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Model.Models.TimeManagement;
using Sofco.DAL.Repositories.Common;
using System;

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
            return base.GetAll();
        }

        private List<Employee> getByEmployeeNumbers(string[] employeeNumbers)
        {
            return context.Employees
                .Where(s => employeeNumbers.Contains(s.EmployeeNumber))
                .ToList();
        }

        public void Save(List<Employee> employees)
        {
            var storedItems = getByEmployeeNumbers(employees.Select(s => s.EmployeeNumber).ToArray());

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
            storedData.Seniority = storedData.Seniority;

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
    }
}
