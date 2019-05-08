using Sofco.Core.DAL.Common;

namespace Sofco.Core.DAL.ManagementReport
{
    public interface IManagementReportRepository : IBaseRepository<Domain.Models.ManagementReport.ManagementReport>
    {
        Sofco.Domain.Models.ManagementReport.ManagementReport GetById(int IdManamentReport);
    }
}
