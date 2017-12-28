using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Core.Cache;
using Sofco.Core.Config;
using Sofco.Core.Data.Billing;
using Sofco.Domain.Crm;
using Sofco.Domain.Crm.Billing;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Data.Billing
{
    public class ProjectData : IProjectData
    {
        private const string ProjectsCacheKey = "urn:services:{0}:projects:{1}:all";
        private const string HitosCacheKey = "urn:hitos:project:{0}:all";
        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;
        private readonly ICrmHttpClient client;
        private readonly CrmConfig crmConfig;

        public ProjectData(ICacheManager cacheManager, ICrmHttpClient client, IOptions<CrmConfig> crmOptions)
        {
            this.cacheManager = cacheManager;
            this.client = client;
            this.crmConfig = crmOptions.Value;
        }

        public IList<CrmProject> GetProjects(string serviceId, string userName, string userMail, bool hasDirectorGroup)
        {
            return cacheManager.GetHashList(string.Format(ProjectsCacheKey, userName, serviceId),
                () =>
                {
                    var url = hasDirectorGroup
                        ? $"{crmConfig.Url}/api/project?idService={serviceId}"
                        : $"{crmConfig.Url}/api/project?idService={serviceId}&idManager={userMail}";

                    var result = client.GetMany<CrmProject>(url);

                    return result.Data;
                },
                x => x.Id,
                cacheExpire);
        }

        public IList<CrmProjectHito> GetHitos(string projectId, bool reload)
        {
            if (reload)
            {
                var hitos = GetHitosFromCrm(projectId);
                cacheManager.SetHashList(string.Format(HitosCacheKey, projectId), hitos, x => x.Id, cacheExpire);
                return hitos;
            }

            return cacheManager.GetHashList(string.Format(HitosCacheKey, projectId),
                () => GetHitosFromCrm(projectId),
                x => x.Id,
                cacheExpire);
        }

        private IList<CrmProjectHito> GetHitosFromCrm(string projectId)
        {
            var url = $"{crmConfig.Url}/api/InvoiceMilestone?idProject={projectId}";

            var result = client.GetMany<CrmProjectHito>(url);

            return result.Data;
        }
    }
}
