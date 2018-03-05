using System;
using Sofco.Core.DAL.Common;
using System.Collections.Generic;
using Sofco.Model.DTO;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IAllocationRepository : IBaseRepository<Allocation>
    {
        ICollection<Allocation> GetAllocationsBetweenDays(int employeeId, DateTime startDate, DateTime endDate);
        void UpdatePercentage(Allocation allocation);
        void UpdateReleaseDate(Allocation allocation);
        ICollection<Employee> GetByService(string serviceId);
        ICollection<Allocation> GetByEmployee(int id);
        ICollection<Employee> GetByEmployeesForReport(AllocationReportParams parameters);
        IList<decimal> GetAllPercentages();
        void RemoveAllocationByAnalytic(int analyticId, DateTime today);
    }
}
