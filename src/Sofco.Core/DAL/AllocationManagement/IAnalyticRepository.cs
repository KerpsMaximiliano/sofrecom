using Sofco.Core.DAL.Common;
using Sofco.Model.Models.TimeManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IAnalyticRepository : IBaseRepository<Analytic>
    {
        bool Exist(int id);
    }
}
