using Sofco.Model.DTO;
using Sofco.Model.Utils;
using Sofco.Model.Models.TimeManagement;
using System;
using System.Collections.Generic;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IAllocationService
    {
        Response<Allocation> Add(AllocationAsignmentParams parameters);
        ICollection<Allocation> GetAllocations(int employeeId, DateTime startDate, DateTime endDate);
        AllocationResponse GetAllocationsBetweenDays(int employeeId, DateTime startDate, DateTime endDate);
    }
}
