using System;
using System.Collections.Generic;
using Sofco.Core.Cache;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Domain.Crm;
using Sofco.Domain.Models.Billing;
using Sofco.Service.Crm.Interfaces;

namespace Sofco.Data.Billing
{
    public class ProjectData : IProjectData
    {
        private const string ProjectsCacheKey = "urn:services:{0}:projects:all";
        private const string HitosCacheKey = "urn:projects:{0}:hitos:all";
        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);
        private readonly TimeSpan hitosCacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICrmInvoicingMilestoneService crmInvoicingMilestoneService;

        public ProjectData(ICacheManager cacheManager, IUnitOfWork unitOfWork, ICrmInvoicingMilestoneService crmInvoicingMilestoneService)
        {
            this.cacheManager = cacheManager;
            this.unitOfWork = unitOfWork;
            this.crmInvoicingMilestoneService = crmInvoicingMilestoneService;
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
            return cacheManager.GetHashList(string.Format(HitosCacheKey, projectId),
                () => crmInvoicingMilestoneService.GetByProjectId(Guid.Parse(projectId)),
            x => x.Id,
                hitosCacheExpire);
        }

        public void ClearKeys()
        {
            cacheManager.DeletePatternKey(string.Format(ProjectsCacheKey, '*'));
        }

        public void ClearHitoKeys(string projectId)
        {
            cacheManager.DeletePatternKey(string.Format(HitosCacheKey, projectId));
            GetHitos(projectId);
        }
    }
}
