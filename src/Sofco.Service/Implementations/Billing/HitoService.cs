using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Billing;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class HitoService : IHitoService
    {
        private readonly CrmConfig crmConfig;
        private readonly ILogMailer<HitoService> logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly IHostingEnvironment environment;

        public HitoService(IOptions<CrmConfig> crmOptions, ILogMailer<HitoService> logger, IUnitOfWork unitOfWork, IHostingEnvironment environment)
        {
            this.crmConfig = crmOptions.Value;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.environment = environment;
        }
         
        public Response Close(string id)
        {
            var response = new Response();

            var closeStatusCode = crmConfig.CloseStatusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);

                try
                {
                    var stringContent = new StringContent("StatusCode="+ closeStatusCode, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var result = client.PutAsync($"/api/InvoiceMilestone/{id}", stringContent).Result;

                    result.EnsureSuccessStatusCode();

                    response.Messages.Add(new Message(Resources.Billing.Solfac.CloseHito, MessageType.Success));
                }
                catch (Exception)
                {
                    response.Messages.Add(new Message(Resources.Billing.Solfac.ErrorSaveOnHitos, MessageType.Error));
                    return response;
                }
            }

            return response;
        }

        public async Task<Response> SplitHito(HitoSplittedParams hito)
        {
            var response = ValidateHitoSplitted(hito);

            if (response.HasErrors()) return response;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);

                await UpdateFirstHito(response, hito, client);
                await CreateNewHito(response, hito, client);

                if (!response.HasErrors())
                {
                    response.AddSuccess(Resources.Billing.Project.HitoSplitted);
                }
            }

            return response;
        }

        public async Task<Response> Create(HitoSplittedParams hito)
        {
            var response = ValidateHitoSplitted(hito);

            var currency = ValdiateCurrency(hito, response);

            if (response.HasErrors()) return response;

            hito.MoneyId = environment.EnvironmentName.Equals("azgap01wp") ? currency.CrmProductionId : currency.CrmDevelopmentId;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);

                await CreateNewHito(response, hito, client);

                if (!response.HasErrors())
                {
                    response.AddSuccess(Resources.Billing.Project.HitoCreated);
                }
            }

            return response;
        }

        private Currency ValdiateCurrency(HitoSplittedParams hito, Response response)
        {
            var currency = unitOfWork.UtilsRepository.GetCurrencies().SingleOrDefault(x => x.Id == Convert.ToInt32(hito.MoneyId));

            if (currency == null)
            {
                response.AddError(Resources.Common.CurrencyRequired);
            }

            return currency;
        }

        private async Task CreateNewHito(Response response, HitoSplittedParams hito, HttpClient client)
        {
            var data =
                $"Ammount={hito.Ammount}&StatusCode=1&StartDate={hito.StartDate:O}&Name={hito.Name}&MoneyId={hito.MoneyId}" +
                $"&Month={hito.Month}&ProjectId={hito.ProjectId}&OpportunityId={hito.OpportunityId}&ManagerId={hito.ManagerId}";

            var urlPath = "/api/InvoiceMilestone";

            try
            {
                var stringContent = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

                var httpResponse = await client.PostAsync(urlPath, stringContent);

                httpResponse.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                logger.LogError(urlPath + "; data: " + data, ex);
                response.AddError(Resources.Billing.Solfac.ErrorSaveOnHitos);
            }
        }

        private async Task UpdateFirstHito(Response response, HitoSplittedParams hito, HttpClient client)
        {
            var closeStatusCode = crmConfig.CloseStatusCode;

            if (hito.AmmountFirstHito == 0 || hito.StatusCode == closeStatusCode) return;

            if (hito.AmmountFirstHito - hito.Ammount <= 0)
                hito.AmmountFirstHito = 0;
            else
                hito.AmmountFirstHito -= hito.Ammount.GetValueOrDefault();

            var data = $"Ammount={hito.AmmountFirstHito}";

            if (hito.AmmountFirstHito == 0) data += "&StatusCode=" + closeStatusCode;

            var urlPath = $"/api/InvoiceMilestone/{hito.ExternalHitoId}";

            try
            {
                var stringContent = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

                var httpResponse = await client.PutAsync(urlPath, stringContent);

                httpResponse.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                logger.LogError(urlPath + "; data: " + data, ex);
                response.AddError(Resources.Billing.Solfac.ErrorSaveOnHitos);
            }
        }

        private Response ValidateHitoSplitted(HitoSplittedParams hito)
        {
            var response = new Response();

            HitoValidatorHelper.ValidateName(hito, response);
            HitoValidatorHelper.ValidateMonth(hito, response);
            HitoValidatorHelper.ValidateAmmounts(hito, response);
            HitoValidatorHelper.ValidateOpportunity(hito, response);

            return response;
        }
    }
}
