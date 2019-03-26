using System;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Services.Billing;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Service.Crm.Interfaces;
using Sofco.Core.Data.Billing;
using Sofco.Core.Logger;

namespace Sofco.Service.Implementations.Billing
{
    public class HitoService : IHitoService
    {
        private readonly CrmConfig crmConfig;
        private readonly ICrmInvoicingMilestoneService crmInvoicingMilestoneService;
        private readonly IProjectData projectData;
        private readonly ILogMailer<HitoService> logger;

        public HitoService(IOptions<CrmConfig> crmOptions,
            IProjectData projectData,
            ILogMailer<HitoService> logger,
            ICrmInvoicingMilestoneService crmInvoicingMilestoneService)
        {
            this.crmConfig = crmOptions.Value;
            this.projectData = projectData;
            this.logger = logger;
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

            this.ClearHitoKeys(hito.ProjectId);

            return response;
        }

        private void ClearHitoKeys(string hitoProjectId)
        {
            try
            {
                this.projectData.ClearHitoKeys(hitoProjectId);
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        public Response Create(HitoParameters hito)
        {
            var response = ValidateHitoSplitted(hito);

            if (response.HasErrors()) return response;

            crmInvoicingMilestoneService.Create(hito, response);

            if (!response.HasErrors())
            {
                this.ClearHitoKeys(hito.ProjectId);
                response.AddSuccess(Resources.Billing.Project.HitoCreated);
            }
            else
            {
                response.AddError(Resources.Billing.Solfac.ErrorSaveOnHitos);
            }

            return response;
        }

        public Response UpdateCurrency(HitoAmmountParameter hito)
        {
            var response = new Response();

            if (string.IsNullOrWhiteSpace(hito.Id))
                response.AddError(Resources.Billing.Solfac.HitoNotFound);

            if (!hito.Ammount.HasValue || hito.Ammount == 0)
                response.AddError(Resources.Billing.Project.HitoAmmoutRequired);

            if (response.HasErrors()) return response;

            crmInvoicingMilestoneService.UpdateAmmount(hito, response);

            if (!response.HasErrors())
                projectData.ClearHitoKeys(hito.ProjectId);

            return response;
        }

        public Response Delete(string hitoId, string projectId)
        {
            var response = new Response();

            if (string.IsNullOrWhiteSpace(hitoId))
            {
                response.AddError(Resources.Billing.Solfac.HitoNotFound);
                return response;
            }

            crmInvoicingMilestoneService.Delete(hitoId, response);

            if (!response.HasErrors())
                projectData.ClearHitoKeys(projectId);

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

            HitoValidatorHelper.ValidateCurrency(hito, response);
            HitoValidatorHelper.ValidateName(hito, response);
            HitoValidatorHelper.ValidateDate(hito, response);
            HitoValidatorHelper.ValidateMonth(hito, response);
            HitoValidatorHelper.ValidateAmmounts(hito, response);
            HitoValidatorHelper.ValidateOpportunity(hito, response);

            return response;
        }
    }
}
