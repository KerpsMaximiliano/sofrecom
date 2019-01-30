using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;

namespace Sofco.Core.Data.WorktimeManagement
{
    public interface IWorktimeData
    {
        IList<TigerReportItem> GetAllTigerReport();

        void ClearTigerReportKey();

        void SaveTigerReport(IList<TigerReportItem> list);

        IList<WorkTimeControlResourceModel> GetAllControlHoursReport(string username);

        void SaveControlHoursReport(IList<WorkTimeControlResourceModel> list, string username);

        void ClearControlHoursReportKey(string username);
    }
}
