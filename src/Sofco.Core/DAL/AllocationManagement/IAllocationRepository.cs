using System;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.TimeManagement;
using System.Collections.Generic;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IAllocationRepository : IBaseRepository<Allocation>
    {
        ICollection<Allocation> GetAllocationsBetweenDays(int employeeId, DateTime startDate, DateTime endDate);
        void UpdatePercentage(Allocation allocation);
        void UpdateReleaseDate(Allocation allocation);
    }
}
