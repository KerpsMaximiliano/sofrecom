using Sofco.Model.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IAnalyticService
    {
        ICollection<Analytic> GetAll();
        Response<Analytic> GetById(int id);
        Response<IList<Allocation>> GetResources(int id);
        AnalyticOptions GetOptions();
        Task<Response<Analytic>> Add(Analytic analytic);
        Response<string> GetNewTitle(int costCenterId);
        Response<Analytic> Update(Analytic domain);
        Response Close(int analyticId);
        ICollection<Analytic> GetAllActives();
    }
}
