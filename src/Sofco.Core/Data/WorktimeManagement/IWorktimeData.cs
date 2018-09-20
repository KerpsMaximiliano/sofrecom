using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;

namespace Sofco.Core.Data.WorktimeManagement
{
    public interface IWorktimeData
    {
        IList<TigerReportItem> GetAll();

        void ClearKeys();

        void SaveTigerReport(IList<TigerReportItem> list);
    }
}
