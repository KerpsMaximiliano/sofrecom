using Sofco.Core.Models.ManagementReport;
using Sofco.Domain.Crm;
using Sofco.Domain.DTO;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IHitoService
    {
        Response Close(string id);

        Response<string> SplitHito(HitoParameters hito);

        Response<string> Create(HitoParameters hito);

        Response Patch(UpdateResourceBillingRequest hito);

        Response Delete(string hitoId, string projectId);

        Response<CrmProjectHito> Get(string id);
    }
}
