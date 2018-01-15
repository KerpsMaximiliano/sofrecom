﻿using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        ICollection<Employee> GetAll();

        bool Exist(int employeeId);

        Employee GetById(int id);

        void Save(List<Employee> employees);

        List<Employee> GetByEndDate(DateTime today);

        List<Employee> GetByEmployeeNumber(string[] employeeNumbers);

        Employee GetByEmployeeNumber(string employeeNumber);
        void UpdateEndDate(Employee employeeToChange);
    }
}
