using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Workflow;
using Sofco.Core.Logger;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Workflow;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Workflow
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IWorkflowRepository workflowRepository;
        private readonly ILogMailer<WorkflowService> logger;
        private readonly IUserData userData;
        private readonly IWorkflowConditionStateFactory workflowConditionStateFactory;
        private readonly IUnitOfWork unitOfWork;
        private readonly IWorkflowValidationStateFactory workflowValidationStateFactory;

        public WorkflowService(IWorkflowRepository workflowRepository, 
            ILogMailer<WorkflowService> logger, 
            IUserData userData, 
            IUnitOfWork unitOfWork,
            IWorkflowValidationStateFactory workflowValidationStateFactory,
            IWorkflowConditionStateFactory workflowConditionStateFactory)
        {
            this.workflowRepository = workflowRepository;
            this.logger = logger;
            this.userData = userData;
            this.workflowConditionStateFactory = workflowConditionStateFactory;
            this.workflowValidationStateFactory = workflowValidationStateFactory;
            this.unitOfWork = unitOfWork;
        }

        public Response DoTransition<T>(WorkflowChangeStatusParameters parameters) where T : WorkflowEntity
        {
            var response = new Response();

            // Validate Parameters
            ValidateParameters(parameters, response);

            if (response.HasErrors()) return response;

            var entity = workflowRepository.GetEntity<T>(parameters.EntityId);

            // Validate if entity exist
            if (entity == null)
            {
                response.AddError(Resources.Workflow.Workflow.EntityNull);
                return response;
            }

            var transition = workflowRepository.GetTransition(entity.StatusId, parameters.NextStateId, parameters.WorkflowId);

            // Validate if transition exist
            if (transition == null)
            {
                response.AddError(Resources.Workflow.Workflow.CannotDoTransition);
                return response;
            }

            var currentUser = userData.GetCurrentUser();

            // Validate user access
            if (!ValidateAccess(transition, currentUser, entity))
            {
                response.AddError(Resources.Workflow.Workflow.UserHasNoAccess);
                return response;
            }

            // Custom Validation
            if (!string.IsNullOrWhiteSpace(transition.ValidationCode))
            {
                var validatorHandler = workflowValidationStateFactory.GetInstance(transition.ValidationCode);

                if (!validatorHandler.Validate(entity, response))
                {
                    return response;
                }
            }

            // Save change status
            try
            {
                entity.StatusId = parameters.NextStateId;

                entity.InWorkflowProcess = !workflowRepository.IsEndTransition(parameters.NextStateId, parameters.WorkflowId);

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

        private bool ValidateAccess(Domain.Models.Workflow.WorkflowStateTransition transition, Core.Models.Admin.UserLiteModel currentUser, WorkflowEntity entity)
        {
            bool hasAccess = false;

            hasAccess = ValidateUserAccess(transition, currentUser, hasAccess);
    
            hasAccess = ValidateGroupAccess(transition, currentUser, hasAccess);

            hasAccess = ValidateApplicantAccess(transition, currentUser, entity, hasAccess);
  
            hasAccess = ValidateManagerAccess(transition, currentUser, entity, hasAccess);

            hasAccess = ValidateSectorAccess(transition, currentUser, entity, hasAccess);

            return hasAccess;
        }

        private bool ValidateSectorAccess(WorkflowStateTransition transition, UserLiteModel currentUser, WorkflowEntity entity, bool hasAccess)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.Code == "SECTOR"))
            {
                var employee = unitOfWork.EmployeeRepository.GetByEmail(entity.UserApplicant.Email);

                var sectors = unitOfWork.EmployeeRepository.GetAnalyticsWithSector(employee.Id);

                if (sectors.Any(x => x.ResponsableUserId == currentUser.Id))
                {
                    hasAccess = true;
                }
            }

            return hasAccess;
        }

        private bool ValidateManagerAccess(Domain.Models.Workflow.WorkflowStateTransition transition, Core.Models.Admin.UserLiteModel currentUser, WorkflowEntity entity, bool hasAccess)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.Code == "MANAGER"))
            {
                if (entity.AuthorizerId.HasValue && entity.AuthorizerId.Value == currentUser.Id)
                {
                    hasAccess = true;
                }
                else
                {
                    var employee = unitOfWork.EmployeeRepository.GetByEmail(entity.UserApplicant.Email);

                    if (employee.ManagerId.HasValue && employee.Manager != null && employee.ManagerId.Value == currentUser.Id)
                    {
                        hasAccess = true;
                    }
                }
            }

            return hasAccess;
        }

        private bool ValidateApplicantAccess(Domain.Models.Workflow.WorkflowStateTransition transition, Core.Models.Admin.UserLiteModel currentUser, WorkflowEntity entity, bool hasAccess)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.Code == "APPLICANT" && entity.UserApplicantId == currentUser.Id))
            {
                hasAccess = true;
            }

            return hasAccess;
        }

        private bool ValidateGroupAccess(Domain.Models.Workflow.WorkflowStateTransition transition, Core.Models.Admin.UserLiteModel currentUser, bool hasAccess)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.Code == "GROUP"))
            {
                var user = unitOfWork.UserRepository.GetSingleWithUserGroup(x => x.Id == currentUser.Id);
                var groups = user.UserGroups?.Select(x => x.Group).Distinct().ToList();

                if (groups != null && groups.Any())
                {
                    if (transition.WorkflowStateAccesses.Any(x => groups.Any(u => u.Id == x.UserSource.SourceId) && x.UserSource.Code == "GROUP"))
                    {
                        hasAccess = true;
                    }
                }
            }

            return hasAccess;
        }

        private bool ValidateUserAccess(Domain.Models.Workflow.WorkflowStateTransition transition, Core.Models.Admin.UserLiteModel currentUser, bool hasAccess)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.SourceId == currentUser.Id && x.UserSource.Code == "USER"))
            {
                hasAccess = true;
            }

            return hasAccess;
        }

        private void ValidateParameters(WorkflowChangeStatusParameters parameters, Response response)
        {
            if (parameters == null)
            {
                response.AddError(Resources.Workflow.Workflow.ParametersNull);
            }
        }

        public Response<IList<TransitionItemModel>> GetPossibleTransitions<T>(TransitionParameters parameters) where T : WorkflowEntity
        {
            var response = new Response<IList<TransitionItemModel>>();

            if (parameters == null)
            {
                response.AddError(Resources.Workflow.Workflow.TransitionParametersNull);
                return response;
            }

            response.Data = new List<TransitionItemModel>();

            var currentUser = userData.GetCurrentUser();

            var transitions = workflowRepository.GetTransitions(parameters.ActualStateId, parameters.WorkflowId);

            var entity = workflowRepository.GetEntity<T>(parameters.EntityId);

            if (entity == null)
            {
                response.AddError(Resources.Workflow.Workflow.EntityNull);
                return response;
            }

            foreach (var transition in transitions)
            {
                if (ValidateAccess(transition, currentUser, entity))
                {
                    if (!string.IsNullOrWhiteSpace(transition.ConditionCode))
                    {
                        var conditionHandler = workflowConditionStateFactory.GetInstance(transition.ConditionCode);

                        if (conditionHandler.CanDoTransition(entity, response))
                        {
                            AddTransition(response, transition);
                        }
                    }
                    else
                    {
                        AddTransition(response, transition);
                    }
                }
            }

            return response;
        }

        private void AddTransition(Response<IList<TransitionItemModel>> response, Domain.Models.Workflow.WorkflowStateTransition transition)
        {
            response.Data.Add(new TransitionItemModel
            {
                WorkflowId = transition.WorkflowId,
                NextStateId = transition.NextWorkflowStateId,
                NextStateDescription = transition.NextWorkflowState.Name,
                WorkFlowStateType = transition.NextWorkflowState.Type
            });
        }
    }
}
