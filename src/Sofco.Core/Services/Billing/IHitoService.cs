using System.Threading.Tasks;
using Sofco.Model.DTO;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IHitoService
    {
        Response Close(string id);

        Task<Response> SplitHito(HitoSplittedParams hito);

        Task<Response> Create(HitoSplittedParams hito);
    }
}
