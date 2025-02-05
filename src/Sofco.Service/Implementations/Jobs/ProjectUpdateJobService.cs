﻿using System;
using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Crm;
using Sofco.Domain.Models.Billing;
using Sofco.Service.Crm.Interfaces;

namespace Sofco.Service.Implementations.Jobs
{
    public class ProjectUpdateJobService : IProjectUpdateJobService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<ProjectUpdateJobService> logger;
        private readonly IProjectData projectData;
        private readonly ICrmProjectService crmProjectService;
        private readonly IMapper mapper;

        public ProjectUpdateJobService(IUnitOfWork unitOfWork,
            IProjectData projectData,
            ILogMailer<ProjectUpdateJobService> logger, 
            ICrmProjectService crmProjectService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.crmProjectService = crmProjectService;
            this.mapper = mapper;
            this.projectData = projectData;
        }

        public void Execute()
        {
            var result = crmProjectService.GetAll();

            foreach (var crmProject in result)
            {
                var project = unitOfWork.ProjectRepository.GetByIdCrm(crmProject.Id);

                if (project == null)
                    Create(crmProject);
                else
                    Update(crmProject, project);
            }

            try
            {
                unitOfWork.Save();
                projectData.ClearKeys();
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        private void Update(CrmProject crmProject, Project project)
        {
            try
            {
                unitOfWork.ProjectRepository.Update(Translate(crmProject, project));
            }
            catch (Exception e)
            {
                logger.LogError($"Error on update project: {crmProject.Name}", e);
            }
        }

        private void Create(CrmProject crmProject)
        {
            try
            {
                unitOfWork.ProjectRepository.Insert(Translate(crmProject));
            }
            catch (Exception e)
            {
                logger.LogError($"Error on insert project: {crmProject.Name}", e);
            }
        }

        private Project Translate(CrmProject crmProject, Project project = null)
        {
            project = project ?? new Project();

            mapper.Map(crmProject, project);

            project.Active = crmProject.StateCode == 0;

            return project;
        }
    }
}
