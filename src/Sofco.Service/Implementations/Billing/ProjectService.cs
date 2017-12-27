using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sofco.Core.Config;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
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
        private readonly IUnitOfWork unitOfWork;
        private readonly CrmConfig crmConfig;
        private readonly ICrmHttpClient client;
        private readonly IProjectData projectData;

        public ProjectService(ISolfacService solfacService, 
            IOptions<CrmConfig> crmOptions, 
            IUnitOfWork unitOfWork,
            IProjectData projectData, 
            ICrmHttpClient client)
        {
            this.solfacService = solfacService;
            this.unitOfWork = unitOfWork;
            crmConfig = crmOptions.Value;
            this.projectData = projectData;
            this.client = client;
        }

        public IList<CrmProjectHito> GetHitosByProject(string projectId)
        {
            var hitos = solfacService.GetHitosByProject(projectId);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);
                var response = client.GetAsync($"/api/InvoiceMilestone?idProject={projectId}").Result;
                response.EnsureSuccessStatusCode();

                var stringResult = response.Content.ReadAsStringAsync().Result;

                var crmProjectHitos = JsonConvert.DeserializeObject<IList<CrmProjectHito>>(stringResult);

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
        }

        public IList<CrmProject> GetProjects(string serviceId, string userMail, string userName)
        {
            var hasDirectorGroup = this.unitOfWork.UserRepository.HasDirectorGroup(userMail);

            return projectData.GetProjects(serviceId, userName, userMail, hasDirectorGroup);
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
    }
}
