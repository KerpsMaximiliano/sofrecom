using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Core.DAL.Rrhh
{
    public interface IPrepaidImportedDataRepository : IBaseRepository<PrepaidImportedData>
    {
        IList<PrepaidDashboard> GetDashboard(int yearId, int monthId);
    }
}
