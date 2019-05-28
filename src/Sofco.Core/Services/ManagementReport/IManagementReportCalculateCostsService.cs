using System;
using Sofco.Domain.DTO;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.ManagementReport
{
    public interface IManagementReportCalculateCostsService
    {
        void CalculateCosts(AllocationDto allocationDto, DateTime firstMonthDate, DateTime lastMonthDate);
        void UpdateManagementReports(Response response, int year, int month);
    }
}
