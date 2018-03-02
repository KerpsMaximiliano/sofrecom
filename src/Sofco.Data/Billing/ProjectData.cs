using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Cache;
using Sofco.Core.Config;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL.Admin;
using Sofco.Domain.Crm;
using Sofco.Domain.Crm.Billing;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Data.Billing
{
    public class ProjectData : IProjectData
    {
        private const string ProjectsCacheKey = "urn:services:{0}:projects:{1}:all";
        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly IUserRepository userRepository;
        private readonly ICacheManager cacheManager;
        private readonly ISessionManager sessionManager;
        private readonly ICrmHttpClient client;
        private readonly CrmConfig crmConfig;

        public ProjectData(ICacheManager cacheManager, ICrmHttpClient client, IOptions<CrmConfig> crmOptions, IUserRepository userRepository, ISessionManager sessionManager)
        {
            this.cacheManager = cacheManager;
            this.client = client;
            this.userRepository = userRepository;
            this.sessionManager = sessionManager;
            crmConfig = crmOptions.Value;
        }

        public IList<CrmProject> GetProjects(string serviceId, string userMail)
        {
            var email = sessionManager.GetUserEmail(userMail);

            var userName = email.Split('@')[0];

            return cacheManager.GetHashList(string.Format(ProjectsCacheKey, userName, serviceId),
                () =>
                {
                    var hasDirectorGroup = userRepository.HasDirectorGroup(email);
                    var hasCommercialGroup = userRepository.HasComercialGroup(email);
                    var hasAllAccess = hasDirectorGroup || hasCommercialGroup;

                    var url = hasAllAccess
                        ? $"{crmConfig.Url}/api/project?idService={serviceId}"
                        : $"{crmConfig.Url}/api/project?idService={serviceId}&idManager={email}";

                    var result = client.GetMany<CrmProject>(url);

                    return result.Data;
                },
                x => x.Id,
                cacheExpire);
        }

        public IList<CrmProjectHito> GetHitos(string projectId)
        {
            return GetHitosFromCrm(projectId);
        }

        private IList<CrmProjectHito> GetHitosFromCrm(string projectId)
        {
            var url = $"{crmConfig.Url}/api/InvoiceMilestone?idProject={projectId}";

            var result = client.GetMany<CrmProjectHito>(url);

            return result.Data;
        }
    }
}
