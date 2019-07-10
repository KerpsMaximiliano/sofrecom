using Sofco.Core.DAL.Common;
using Sofco.Core.Models.ManagementReport;
using System;
using System.Collections.Generic;

namespace Sofco.Core.DAL.ManagementReport
{
    public interface IManagementReportRepository : IBaseRepository<Domain.Models.ManagementReport.ManagementReport>
    {
        Domain.Models.ManagementReport.ManagementReport GetById(int IdManamentReport);
        Domain.Models.ManagementReport.ManagementReport GetWithAnalytic(int managementReportId);
        void UpdateStatus(Domain.Models.ManagementReport.ManagementReport report);
        bool AllMonthsAreClosed(int managementReportId);
        List<Domain.Models.ManagementReport.ManagementReport> GetByDate(DateTime date);
    }
}
