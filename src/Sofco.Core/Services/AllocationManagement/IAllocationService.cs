using Sofco.Model.DTO;
using Sofco.Model.Utils;
using System;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IAllocationService
    {
        Response<Allocation> Add(AllocationDto allocation);
        AllocationResponse GetAllocationsBetweenDays(int employeeId, DateTime startDate, DateTime endDate);
    }
}
