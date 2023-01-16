using Sofco.Domain.DTO;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IAllocationService
    {
        Response<Allocation> Add(AllocationDto allocation);
        AllocationResponse GetAllocationsBetweenDays(int employeeId, DateTime startDate, DateTime endDate, IList<int> analyticIds);
        ICollection<Employee> GetByService(string serviceId);
        Response<AllocationReportModel> CreateReport(AllocationReportParams parameters);
        IEnumerable<OptionPercentage> GetAllPercentages();
        ICollection<Employee> GetByEmployeesByAnalytic(int analyticId);
        Response<byte[]> AddMassive(AllocationMassiveAddModel model);
        Response<String> GetSectorByEmployee(int employeeId, DateTime startDate, DateTime endDate);
    }
}
