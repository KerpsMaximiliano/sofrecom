using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.WorkTimeManagement
{
    public interface IWorkTimeReportService
    {
        Response<WorkTimeReportModel> CreateReport(ReportParams parameters);
    }
}
