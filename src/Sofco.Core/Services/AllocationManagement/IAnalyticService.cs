using Sofco.Model.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sofco.Core.Models.Billing;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IAnalyticService
    {
        ICollection<Analytic> GetAll();
        Response<Analytic> GetById(int id);
        Response<IList<Allocation>> GetTimelineResources(int id);
        AnalyticOptions GetOptions();
        Task<Response<Analytic>> Add(Analytic analytic);
        Response<string> GetNewTitle(int costCenterId);
        Response<Analytic> Update(Analytic domain);
        Response Close(int analyticId);
        ICollection<Analytic> GetAllActives();
        ICollection<AnalyticOptionForOcModel> GetByClient(string clientId);
        IList<Option> GetResources(int id);
    }
}
