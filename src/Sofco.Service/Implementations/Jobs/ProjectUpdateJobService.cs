using System;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Crm.Billing;
using Sofco.Domain.Models.Billing;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Service.Implementations.Jobs
{
    public class ProjectUpdateJobService : IProjectUpdateJobService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICrmHttpClient client;
        private readonly CrmConfig crmConfig;
        private readonly ILogMailer<ProjectUpdateJobService> logger;
        private readonly IProjectData projectData;

        public ProjectUpdateJobService(IUnitOfWork unitOfWork,
            ICrmHttpClient client,
            IProjectData projectData,
            IOptions<CrmConfig> crmOptions,
            ILogMailer<ProjectUpdateJobService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.client = client;
            crmConfig = crmOptions.Value;
            this.logger = logger;
            this.projectData = projectData;
        }

        public void Execute()
        {
            var url = $"{crmConfig.Url}/api/project";

            var result = client.GetMany<CrmProject>(url);

            unitOfWork.BeginTransaction();

            foreach (var crmProject in result.Data)
            {
                var project = unitOfWork.ProjectRepository.GetByIdCrm(crmProject.Id);

                if (project == null)
                    Create(crmProject);
                else
                    Update(crmProject, project);
            }

            try
            {
                unitOfWork.Commit();
                projectData.ClearKeys();
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        private void Update(CrmProject crmProject, Project project)
        {
            FillData(project, crmProject);

            try
            {
                unitOfWork.ProjectRepository.Update(project);
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError($"Error modificacion proyecto: {crmProject.Nombre}", e);
            }
        }

        private void Create(CrmProject crmProject)
        {
            var project = new Project();

            FillData(project, crmProject);

            try
            {
                unitOfWork.ProjectRepository.Insert(project);
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError($"Error alta proyecto: {crmProject.Nombre}", e);
            }
        }

        private void FillData(Project project, CrmProject crmProject)
        {
            project.CrmId = crmProject.Id;
            project.Name = crmProject.Nombre;
            project.AccountId = crmProject.AccountId;
            project.ServiceId = crmProject.ServiceId;
            project.StartDate = crmProject.StartDate;
            project.EndDate = crmProject.EndDate;
            project.Incomes = crmProject.Incomes;
            project.RealIncomes = crmProject.RealIncomes;
            project.OpportunityId = crmProject.OpportunityId;
            project.OpportunityName = crmProject.OpportunityName;
            project.OpportunityNumber = crmProject.OpportunityNumber;
            project.TotalAmmount = crmProject.TotalAmmount;
            project.Currency = crmProject.Currency;
            project.CurrencyId = crmProject.CurrencyId;
            project.Remito = crmProject.Remito;
            project.Integrator = crmProject.Integrator;
            project.IntegratorId = crmProject.IntegratorId;
            project.Active = true;
        }
    }
}
