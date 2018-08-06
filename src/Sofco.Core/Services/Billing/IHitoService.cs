using System.Threading.Tasks;
using Sofco.Domain.DTO;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IHitoService
    {
        Response Close(string id);

        Task<Response> SplitHito(HitoSplittedParams hito);

        Task<Response> Create(HitoSplittedParams hito);
    }
}
