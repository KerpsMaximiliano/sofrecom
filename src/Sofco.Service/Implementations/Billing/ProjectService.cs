using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Data.Billing;
using Sofco.Core.Logger;
using Sofco.Core.Models;
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Crm;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.Enums;
using Sofco.Model.Utils;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Service.Implementations.Billing
{
    public class ProjectService : IProjectService
    {
        private readonly ISolfacService solfacService;
        private readonly CrmConfig crmConfig;
        private readonly ICrmHttpClient client;
        private readonly IProjectData projectData;
        private readonly ILogMailer<ProjectService> logger;

        public ProjectService(ISolfacService solfacService, IOptions<CrmConfig> crmOptions,
            IProjectData projectData, ICrmHttpClient client, ILogMailer<ProjectService> logger)
        {
            this.solfacService = solfacService;
            crmConfig = crmOptions.Value;
            this.projectData = projectData;
            this.client = client;
            this.logger = logger;
        }

        public IList<CrmProjectHito> GetHitosByProject(string projectId)
        {
            var hitos = solfacService.GetHitosByProject(projectId);

            var crmProjectHitos = projectData.GetHitos(projectId);

            foreach (var hitoCrm in crmProjectHitos)
            {
                var existHito = hitos.SingleOrDefault(x => x.ExternalHitoId == hitoCrm.Id);

                if (existHito != null)
                {
                    hitoCrm.SolfacId = existHito.SolfacId;
                }

                if (!hitoCrm.Status.Equals("Pendiente")
                    && !hitoCrm.Status.Equals("Proyectado") || existHito != null)
                {
                    hitoCrm.Billed = true;
                }

                if (hitoCrm.Status.Equals("A ser facturado"))
                {
                    hitoCrm.Status = HitoStatus.ToBeBilled.ToString();
                }
            }

            return crmProjectHitos;
        }

        public Response<IList<CrmProject>> GetProjects(string serviceId)
        {
            var response = new Response<IList<CrmProject>>();

            if (string.IsNullOrWhiteSpace(serviceId)) return response;

            try
            {
                response.Data = projectData.GetProjects(serviceId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }

            return response;
        }

        public Response<IList<SelectListModel>> GetProjectsOptions(string serviceId)
        {
            var result = GetProjects(serviceId).Data;

            return new Response<IList<SelectListModel>>
            {
                Data = result
                    .Select(x => new SelectListModel { Id = x.Id, Text = x.Nombre })
                    .OrderBy(x => x.Text)
                    .ToList()
            };
        }

        public Response<CrmProject> GetProjectById(string projectId)
        {
            var url = $"{crmConfig.Url}/api/project/{projectId}";

            var project = client.Get<CrmProject>(url).Data;

            var response = new Response<CrmProject>();

            if (project.Id.Equals("00000000-0000-0000-0000-000000000000"))
            {
                response.Messages.Add(new Message(Resources.Billing.Project.NotFound, MessageType.Error));
                return response;
            }

            response.Data = project;
            return response;
        }

        public Response<IList<OpportunityOption>> GetOpportunities(string serviceId)
        {
            var result = GetProjects(serviceId).Data;

            return new Response<IList<OpportunityOption>>
            {
                Data = result
                    .Select(x => new OpportunityOption { Id = x.OpportunityId, Text = $"{x.OpportunityNumber} - {x.OpportunityName}", ProjectId = x.Id })
                    .OrderBy(x => x.Text)
                    .ToList()
            };
        }
    }
}
