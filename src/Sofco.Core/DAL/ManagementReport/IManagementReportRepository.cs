using Sofco.Core.DAL.Common;

namespace Sofco.Core.DAL.ManagementReport
{
    public interface IManagementReportRepository : IBaseRepository<Domain.Models.ManagementReport.ManagementReport>
    {
        Domain.Models.ManagementReport.ManagementReport GetById(int IdManamentReport);
        Domain.Models.ManagementReport.ManagementReport GetWithAnalytic(int managementReportId);
        void UpdateStatus(Domain.Models.ManagementReport.ManagementReport report);
    }
}
