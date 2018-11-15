using System;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.Logger;
using Sofco.Core.Managers;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Managers
{
    public class AnalyticManager : IAnalyticManager
    {
        private readonly ILogMailer<AnalyticManager> logger;

        private readonly CrmConfig crmConfig;

        private readonly IUserData userData;

        public AnalyticManager(ILogMailer<AnalyticManager> logger, IOptions<CrmConfig> crmOptions, IUserData userData)
        {
            this.logger = logger;
            this.userData = userData;
            crmConfig = crmOptions.Value;
        }

        public Response UpdateCrmAnalytic(Analytic analytic)
        {
            var response = new Response();

            var analyticTitle = analytic.Title;

            var serviceId = analytic.ServiceId;

            var managerId = GetExternalManagerId(analytic);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);

                var data = new StringBuilder($"Analytic={analyticTitle}");

                if (managerId != null)
                {
                    data.Append($"&ManagerId={managerId}");
                }

                var urlPath = $"/api/Service/{serviceId}/";

                try
                {
                    var stringContent = new StringContent(data.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");

                    var httpResponse = client.PutAsync(urlPath, stringContent).Result;

                    httpResponse.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    logger.LogError(urlPath + "; data: " + data, ex);
                    response.AddError(Resources.Common.ErrorSave);
                }
            }

            return response;
        }

        private Guid? GetExternalManagerId(Analytic analytic)
        {
            if (!analytic.ManagerId.HasValue) return null;

            var managerId = analytic.ManagerId.Value;

            var user = userData.GetById(managerId);

            if (user == null || string.IsNullOrEmpty(user.ExternalManagerId)) return null;

            return Guid.Parse(user.ExternalManagerId);
        }
    }
}
