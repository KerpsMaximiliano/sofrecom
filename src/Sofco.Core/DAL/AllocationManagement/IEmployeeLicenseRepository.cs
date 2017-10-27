using System;
using System.Collections.Generic;
using Sofco.Model.Models.TimeManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeLicenseRepository
    {
        void Save(List<EmployeeLicense> employeeLicense);

        void Delete(List<EmployeeLicense> employeeLicenses, DateTime startDate);
    }
}
