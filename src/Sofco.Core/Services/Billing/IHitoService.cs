using Sofco.Domain.DTO;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IHitoService
    {
        Response Close(string id);

        Response SplitHito(HitoParameters hito);

        Response Create(HitoParameters hito);

        Response UpdateCurrency(HitoAmmountParameter hito);

        Response Delete(string hitoId, string projectId);
    }
}
