using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Services.Billing;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Service.Crm.Interfaces;
using Sofco.Core.DAL;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System;

namespace Sofco.Service.Implementations.Billing
{
    public class HitoService : IHitoService
    {
        private readonly CrmConfig crmConfig;
        private readonly ICrmInvoicingMilestoneService crmInvoicingMilestoneService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IHostingEnvironment environment;

        public HitoService(IOptions<CrmConfig> crmOptions,
            IUnitOfWork unitOfWork,
            IHostingEnvironment environment,
            ICrmInvoicingMilestoneService crmInvoicingMilestoneService)
        {
            this.crmConfig = crmOptions.Value;
            this.environment = environment;
            this.unitOfWork = unitOfWork;
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

        public Response SplitHito(HitoParameters hito)
        {
            var response = Create(hito);
            UpdateFirstHito(response, hito);

            if (response.HasErrors())
            {
                response.AddError(Resources.Billing.Solfac.ErrorSaveOnHitos);
            }

            return response;
        }

        public Response Create(HitoParameters hito)
        {
            var response = ValidateHitoSplitted(hito);

            var currency = ValdiateCurrency(hito, response);

            if (response.HasErrors()) return response;

            hito.MoneyId = environment.EnvironmentName.Equals("azgap01wp") ? currency.CrmProductionId : currency.CrmDevelopmentId;

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

        private void UpdateFirstHito(Response response, HitoParameters hito)
        {
            if (hito.AmmountFirstHito == 0 || hito.StatusCode == crmConfig.CloseStatusCode) return;

            if (hito.AmmountFirstHito - hito.Ammount <= 0)
            {
                hito.StatusCode = crmConfig.CloseStatusCode;
                hito.AmmountFirstHito = 0;
            }
            else
                hito.AmmountFirstHito -= hito.Ammount.GetValueOrDefault();

            crmInvoicingMilestoneService.UpdateAmmountAndStatus(hito, response);
        }

        private Response ValidateHitoSplitted(HitoParameters hito)
        {
            var response = new Response();

            HitoValidatorHelper.ValidateName(hito, response);
            HitoValidatorHelper.ValidateDate(hito, response);
            HitoValidatorHelper.ValidateMonth(hito, response);
            HitoValidatorHelper.ValidateAmmounts(hito, response);
            HitoValidatorHelper.ValidateOpportunity(hito, response);

            return response;
        }

        private Currency ValdiateCurrency(HitoParameters hito, Response response)
        {
            var currency = unitOfWork.UtilsRepository.GetCurrencies().SingleOrDefault(x => x.Id == Convert.ToInt32(hito.MoneyId));

            if (currency == null)
            {
                response.AddError(Resources.Common.CurrencyRequired);
            }

            return currency;
        }
    }
}
