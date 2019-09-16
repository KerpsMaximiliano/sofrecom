using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.Services.Billing;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Service.Crm.Interfaces;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.ManagementReport;
using Sofco.Core.Services.ManagementReport;
using Sofco.Domain.Crm;

namespace Sofco.Service.Implementations.Billing
{
    public class HitoService : IHitoService
    {
        private readonly CrmConfig crmConfig;
        private readonly ICrmInvoicingMilestoneService crmInvoicingMilestoneService;
        private readonly IManagementReportBillingService managementReportBillingService;
        private readonly IProjectData projectData;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<HitoService> logger;
        private readonly AppSetting appSetting;

        public HitoService(IOptions<CrmConfig> crmOptions,
            IProjectData projectData,
            IManagementReportBillingService managementReportBillingService,
            IUnitOfWork unitOfWork,
            ILogMailer<HitoService> logger,
            IOptions<AppSetting> appSettingOptions,
            ICrmInvoicingMilestoneService crmInvoicingMilestoneService)
        {
            this.crmConfig = crmOptions.Value;
            this.projectData = projectData;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.crmInvoicingMilestoneService = crmInvoicingMilestoneService;
            this.managementReportBillingService = managementReportBillingService;
            this.appSetting = appSettingOptions.Value;
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

        public Response<string> SplitHito(HitoParameters hito)
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

        public Response<string> Create(HitoParameters hito)
        {
            var response = ValidateParameters(hito);

            if (response.HasErrors()) return response;

            response.Data = crmInvoicingMilestoneService.Create(hito, response);

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

        public Response<ResourceBillingRequestItem> Patch(UpdateResourceBillingRequest data)
        {
            var response = new Response<ResourceBillingRequestItem>();

            if (string.IsNullOrWhiteSpace(data.Id))
                response.AddError(Resources.Billing.Solfac.HitoNotFound);

            if (!data.Ammount.HasValue || data.Ammount == 0)
                response.AddError(Resources.Billing.Project.HitoAmmoutRequired);

            if(string.IsNullOrWhiteSpace(data.Name))
                response.AddError(Resources.Billing.Project.NameRequired);

            if (response.HasErrors()) return response;

            var responseResources = managementReportBillingService.ValidateAddResources(data.BillingMonthId, data.Resources);

            if (responseResources.HasErrors())
            {
                response.Messages = responseResources.Messages;
                return response;
            }

            if (data.Resources.Any())
            {
                response = managementReportBillingService.AddResources(data.BillingMonthId, data.Resources, data.Id);

                if (response.HasErrors()) return response;
            }

            crmInvoicingMilestoneService.UpdateAmmountAndName(new HitoAmmountParameter(data.Id, data.ProjectId, data.Ammount.GetValueOrDefault(), data.Name, data.Month), response);

            if (!response.HasErrors())
            {
                projectData.ClearHitoKeys(data.ProjectId);
                response.AddSuccess(Resources.Billing.Solfac.HitoUpdateSuccess);
            }
            else
            {
                response.Messages.Clear();
                response.AddError(Resources.Billing.Solfac.ErrorSaveOnHitos);
            }

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

        public Response<CrmProjectHito> Get(string id)
        {
            var response = new Response<CrmProjectHito>();

            var hito = crmInvoicingMilestoneService.GetById(id);

            if (hito == null)
            {
                response.AddError(Resources.Billing.Solfac.HitoNotFound);
            }
            else
            {
                var currencies = unitOfWork.UtilsRepository.GetCurrencies();
                var currency = currencies.SingleOrDefault(x => x.CrmId.Equals(hito.MoneyId));

                if (appSetting.CurrencyPesos != currency?.Id)
                {
                    var currencyExchange = unitOfWork.CurrencyExchangeRepository.Get(hito.StartDate, hito.MoneyId);

                    if (currencyExchange != null)
                    {
                        hito.BaseAmount = hito.Ammount * currencyExchange.Exchange;
                        hito.BaseAmountOriginal = hito.AmountOriginal * currencyExchange.Exchange;
                        response.Data = hito;
                    }
                    else
                    {
                        response.AddError(Resources.ManagementReport.CurrencyExchange.NotFound);
                        return response;
                    }
                }
                else
                {
                    response.Data = hito;
                }
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

        private Response<string> ValidateParameters(HitoParameters hito)
        {
            var response = new Response<string>();

            HitoValidatorHelper.ValidateProject(hito, response);
            HitoValidatorHelper.ValidateCurrency(hito, response);
            HitoValidatorHelper.ValidateName(hito, response);
            HitoValidatorHelper.ValidateDate(hito, response, unitOfWork);
            HitoValidatorHelper.ValidateMonth(hito, response);
            HitoValidatorHelper.ValidateAmmounts(hito, response);
            HitoValidatorHelper.ValidateOpportunity(hito, response);

            return response;
        }
    }
}
