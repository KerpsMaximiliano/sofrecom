using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        new ICollection<Employee> GetAll();

        bool Exist(int employeeId);

        Employee GetById(int id);

        Employee GetByEmail(string email);

        void Update(List<Employee> employees);

        List<Employee> GetByEndDate(DateTime today);

        List<Employee> GetByEmployeeNumber(string[] employeeNumbers);

        Employee GetByEmployeeNumber(string employeeNumber);

        void UpdateEndDate(Employee employeeToChange);

        ICollection<Employee> Search(EmployeeSearchParams parameters);

        void ResetAllExamDays();

        IList<EmployeeCategory> GetEmployeeCategories(int employeeId);

        void UpdateBusinessHours(Employee employee);

        IList<Employee> GetByEmployeeNumbers(IEnumerable<string> employeeNumbers);

        IList<Employee> SearchUnemployees(UnemployeeSearchParameters parameters);

        void Save(List<Employee> employees);

        void Save(Employee employee);

        IList<Employee> GetByAnalyticWithWorkTimes(int analyticId);

        Employee GetUserInfo(string email);

        IList<Employee> GetUnassignedBetweenDays(DateTime startDate, DateTime endDate);

        IList<Employee> GetByAnalyticIds(List<int> analyticIds);

        IList<Sector> GetAnalyticsWithSector(int employeeId);

        IList<Employee> GetByManagerId(int managerId);
    }
}
