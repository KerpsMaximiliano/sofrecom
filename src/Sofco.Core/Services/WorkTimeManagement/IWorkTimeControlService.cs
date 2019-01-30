using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.WorkTimeManagement
{
    public interface IWorkTimeControlService
    {
        Response<WorkTimeControlModel> Get(WorkTimeControlParams parameters);

        Response<List<Option>> GetAnalyticOptionsByCurrentManager();

        Response<byte[]> ExportControlHoursReport();
    }
}