using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.DTO;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        new ICollection<Employee> GetAll();

        bool Exist(int employeeId);

        Employee GetById(int id);

        Employee GetByEmail(string email);

        void Save(List<Employee> employees);

        List<Employee> GetByEndDate(DateTime today);

        List<Employee> GetByEmployeeNumber(string[] employeeNumbers);

        Employee GetByEmployeeNumber(string employeeNumber);

        void UpdateEndDate(Employee employeeToChange);

        ICollection<Employee> Search(EmployeeSearchParams parameters);

        void ResetAllExamDays();

        void UpdateHolidaysPending(Employee employeeToModif);

        void UpdateExamDaysTaken(Employee employeeToModif);

        IList<EmployeeCategory> GetEmployeeCategories(int id);
    }
}
