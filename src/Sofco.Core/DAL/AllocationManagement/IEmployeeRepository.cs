using System;
using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeRepository
    {
        ICollection<Employee> GetAll();

        bool Exist(int employeeId);

        Employee GetById(int id);

        void Save(List<Employee> employees);

        List<Employee> GetByEndDate(DateTime today);

        List<Employee> GetByEmployeeNumber(string[] employeeNumbers);
    }
}
