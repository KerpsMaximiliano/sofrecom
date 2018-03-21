using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.Billing;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IAnalyticRepository : IBaseRepository<Analytic>
    {
        bool Exist(int id);

        IList<Allocation> GetResources(int id);

        Analytic GetLastAnalytic(int costCenterId);

        bool ExistTitle(string analyticTitle);

        IList<Analytic> GetAnalyticsByEmployee(int employeeId);

        void Close(Analytic analytic);
        ICollection<Analytic> GetAllOpenReadOnly();
        bool ExistWithService(string serviceId);
        ICollection<Analytic> GetByClient(string clientId);
    }
}
