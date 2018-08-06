using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Core.Cache;
using Sofco.Core.Config;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Domain.Crm;
using Sofco.Domain.Models.Billing;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Data.Billing
{
    public class ProjectData : IProjectData
    {
        private const string ProjectsCacheKey = "urn:services:{0}:projects:all";
        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;
        private readonly ICrmHttpClient client;
        private readonly CrmConfig crmConfig;
        private readonly IUnitOfWork unitOfWork;

        public ProjectData(ICacheManager cacheManager, ICrmHttpClient client, IOptions<CrmConfig> crmOptions, IUnitOfWork unitOfWork)
        {
            this.cacheManager = cacheManager;
            this.client = client;
            crmConfig = crmOptions.Value;
            this.unitOfWork = unitOfWork;
        }

        public IList<Project> GetProjects(string serviceId)
        {
            return cacheManager.GetHashList(string.Format(ProjectsCacheKey, serviceId),
                () => unitOfWork.ProjectRepository.GetAllActives(serviceId),
                x => x.CrmId,
                cacheExpire);
        }

        public IList<CrmProjectHito> GetHitos(string projectId)
        {
            return GetHitosFromCrm(projectId);
        }

        public void ClearKeys()
        {
            cacheManager.DeletePatternKey(string.Format(ProjectsCacheKey, '*'));
        }

        private IList<CrmProjectHito> GetHitosFromCrm(string projectId)
        {
            var url = $"{crmConfig.Url}/api/InvoiceMilestone?idProject={projectId}";

            var result = client.GetMany<CrmProjectHito>(url);

            return result.Data;
        }
    }
}
