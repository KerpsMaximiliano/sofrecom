using System;
using System.Collections.Generic;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeLicenseRepository
    {
        void Save(List<EmployeeLicense> employeeLicense);

        void Delete(List<EmployeeLicense> employeeLicenses, DateTime startDate);
    }
}
