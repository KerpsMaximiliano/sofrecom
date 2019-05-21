using System;
using System.Collections.Generic;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.Services.ManagementReport
{
    public interface IManagementReportCalculateCostsService
    {
        void CalculateCosts(AllocationDto allocationDto, DateTime firstMonthDate, DateTime lastMonthDate);
    }
}
