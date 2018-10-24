using System;
using Sofco.Core.DAL.Common;
using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.AllocationManagement;

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
        void RemoveAllocationByAnalytic(int analyticId, DateTime today);
        ICollection<Allocation> GetAllocationsLiteBetweenDays(int employeeId, DateTime startDate, DateTime endDate);
        ICollection<Allocation> GetAllocationsForWorktimeReport(ReportParams parameters);
        bool Exist(int allocationId);
        ICollection<Employee> GetByAnalyticId(int analyticId);
        void DeleteAllocationWithReleaseDateNull();
        IList<Allocation> GetLastAllocationsForEmployee(int employeeId, DateTime date);
    }
}
