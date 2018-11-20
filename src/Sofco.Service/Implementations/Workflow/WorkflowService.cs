using System;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL.Workflow;
using Sofco.Core.Logger;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Workflow
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IWorkflowRepository workflowRepository;
        private readonly ILogMailer<WorkflowService> logger;
        private readonly IUserData userData;

        public WorkflowService(IWorkflowRepository workflowRepository, ILogMailer<WorkflowService> logger, IUserData userData)
        {
            this.workflowRepository = workflowRepository;
            this.logger = logger;
            this.userData = userData;
        }

        public Response DoTransition<T>(WorkflowChangeStatusParameters parameters) where T : class
        {
            var response = new Response();

            // Validate Parameters
            ValidateParameters(parameters, response);

            if (response.HasErrors()) return response;

            var entity = workflowRepository.GetEntity<T>(parameters.EntityId.GetValueOrDefault());

            // Validate if entity exist
            if (entity == null)
            {
                response.AddError(Resources.Workflow.Workflow.EntityNull);
                return response;
            }

            var wfEntity = (IWorkflowEntity) entity;

            var transition = workflowRepository.GetTransition(wfEntity.StatusId, parameters.NextStateId.GetValueOrDefault(), parameters.WorkflowId.GetValueOrDefault());

            // Validate if transition exist
            if (transition == null)
            {
                response.AddError(Resources.Workflow.Workflow.CannotDoTransition);
                return response;
            }

            var currentUser = userData.GetCurrentUser();

            // Validate user access
            ValidateAccess(response, transition, currentUser);

            if (response.HasErrors()) return response;

            try
            {
                workflowRepository.UpdateStatus(entity);
                workflowRepository.Save();

                response.AddSuccess(Resources.Workflow.Workflow.TransitionSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void ValidateAccess(Response response, Domain.Models.Workflow.WorkflowStateTransition transition, Core.Models.Admin.UserLiteModel currentUser)
        {
            if (!transition.WorkflowStateAccesses.Any(x => x.UserSource.SourceId == currentUser.Id && x.UserSource.Code == "USER"))
            {
                response.AddError(Resources.Workflow.Workflow.UserHasNoAccess);
            }
        }

        private void ValidateParameters(WorkflowChangeStatusParameters parameters, Response response)
        {
            if (parameters == null)
            {
                response.AddError(Resources.Workflow.Workflow.ParametersNull);
                return;
            }

            if (!parameters.EntityId.HasValue || parameters.EntityId.Value <= 0)
            {
                response.AddError(Resources.Workflow.Workflow.EntityNull);
            }

            if (!parameters.NextStateId.HasValue || parameters.NextStateId.Value <= 0)
            {
                response.AddError(Resources.Workflow.Workflow.NextStateNull);
            }

            if (!parameters.WorkflowId.HasValue || parameters.WorkflowId.Value <= 0)
            {
                response.AddError(Resources.Workflow.Workflow.WorkflowNull);
            }
        }
    }
}
