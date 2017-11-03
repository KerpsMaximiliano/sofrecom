using Sofco.Model.DTO;
using Sofco.Model.Utils;
using Sofco.Model.Models.TimeManagement;
using System;
using System.Collections.Generic;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IAllocationService
    {
        Response<Allocation> Add(AllocationDto allocation);
        AllocationResponse GetAllocationsBetweenDays(int employeeId, DateTime startDate, DateTime endDate);
    }
}
