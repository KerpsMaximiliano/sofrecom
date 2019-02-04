using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Services.Billing;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Service.Crm.Interfaces;

namespace Sofco.Service.Implementations.Billing
{
    public class HitoService : IHitoService
    {
        private readonly CrmConfig crmConfig;
        private readonly ICrmInvoicingMilestoneService crmInvoicingMilestoneService;

        public HitoService(IOptions<CrmConfig> crmOptions,
            ICrmInvoicingMilestoneService crmInvoicingMilestoneService)
        {
            this.crmConfig = crmOptions.Value;
            this.crmInvoicingMilestoneService = crmInvoicingMilestoneService;
        }

        public Response Close(string id)
        {
            var response = new Response();

            var closeStatusCode = crmConfig.CloseStatusCode;

            crmInvoicingMilestoneService.Close(response, id, closeStatusCode);

            if (!response.HasErrors())
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.CloseHito, MessageType.Success));
            }
            else
            {
                response.AddError(Resources.Billing.Solfac.ErrorSaveOnHitos);
            }

            return response;
        }

        public Response SplitHito(HitoSplittedParams hito)
        {
            var response = Create(hito);
            UpdateFirstHito(response, hito);

            if (response.HasErrors())
            {
                response.AddError(Resources.Billing.Solfac.ErrorSaveOnHitos);
            }

            return response;
        }

        public Response Create(HitoSplittedParams hito)
        {
            var response = ValidateHitoSplitted(hito);

            if (response.HasErrors()) return response;

            crmInvoicingMilestoneService.Create(hito, response);

            if (!response.HasErrors())
            {
                response.AddSuccess(Resources.Billing.Project.HitoCreated);
            }
            else
            {
                response.AddError(Resources.Billing.Solfac.ErrorSaveOnHitos);
            }

            return response;
        }

        private void UpdateFirstHito(Response response, HitoSplittedParams hito)
        {
            if (hito.AmmountFirstHito == 0 || hito.StatusCode == crmConfig.CloseStatusCode) return;

            if (hito.AmmountFirstHito - hito.Ammount <= 0)
            {
                hito.StatusCode = crmConfig.CloseStatusCode;
                hito.AmmountFirstHito = 0;
            }
            else
                hito.AmmountFirstHito -= hito.Ammount.GetValueOrDefault();

            crmInvoicingMilestoneService.Update(hito, response);
        }

        private Response ValidateHitoSplitted(HitoSplittedParams hito)
        {
            var response = new Response();

            HitoValidatorHelper.ValidateName(hito, response);
            HitoValidatorHelper.ValidateDate(hito, response);
            HitoValidatorHelper.ValidateMonth(hito, response);
            HitoValidatorHelper.ValidateAmmounts(hito, response);
            HitoValidatorHelper.ValidateOpportunity(hito, response);

            return response;
        }
    }
}
