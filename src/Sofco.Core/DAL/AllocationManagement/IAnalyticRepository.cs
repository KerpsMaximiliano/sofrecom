using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IAnalyticRepository : IBaseRepository<Analytic>
    {
        bool Exist(int id);
        IList<Allocation> GetResources(int id);
        Analytic GetLastAnalytic(int costCenterId);
        bool ExistTitle(string analyticTitle);
    }
}
