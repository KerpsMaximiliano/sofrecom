﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Sofco.Common.Settings;
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
        private readonly AppSetting appSetting;
        private readonly IWorkflowNotificationFactory workflowNotificationFactory;

        public WorkflowService(IWorkflowRepository workflowRepository, 
            ILogMailer<WorkflowService> logger, 
            IUserData userData, 
            IUnitOfWork unitOfWork,
            IWorkflowValidationStateFactory workflowValidationStateFactory,
            IOptions<AppSetting> appSettingsOptions,
            IWorkflowNotificationFactory workflowNotificationFactory,
            IWorkflowConditionStateFactory workflowConditionStateFactory)
        {
            this.workflowRepository = workflowRepository;
            this.logger = logger;
            this.userData = userData;
            this.workflowConditionStateFactory = workflowConditionStateFactory;
            this.workflowValidationStateFactory = workflowValidationStateFactory;
            this.unitOfWork = unitOfWork;
            this.appSetting = appSettingsOptions.Value;
            this.workflowNotificationFactory = workflowNotificationFactory;
        }

        public Response DoTransition<TEntity, THistory>(WorkflowChangeStatusParameters parameters)
            where TEntity : WorkflowEntity 
            where THistory : WorkflowHistory
        {
            var response = new Response();

            // Validate Parameters
            ValidateParameters(parameters, response);

            if (response.HasErrors()) return response;

            var entity = workflowRepository.GetEntity<TEntity>(parameters.EntityId);

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

            // Create history
            CreateHistory<TEntity, THistory>(entity, transition, currentUser);

            // Send Notification
            //SendNotification(entity, response, transition);

            return response;
        }

        private void SendNotification(WorkflowEntity entity, Response response, WorkflowStateTransition transition)
        {
            var notificationHandler = workflowNotificationFactory.GetInstance(transition.NotificationCode);

            notificationHandler.Send(entity, response, transition);
        }

        private void CreateHistory<TEntity, THistory>(TEntity entity, WorkflowStateTransition transition, UserLiteModel currentUser)
            where TEntity : WorkflowEntity
            where THistory : WorkflowHistory
        {
            try
            {
                var history = (THistory)Activator.CreateInstance(typeof(THistory));

                history.UserName = currentUser.Name;
                history.CreatedDate = DateTime.UtcNow.Date;
                history.StatusFromId = transition.ActualWorkflowStateId;
                history.StatusToId = transition.NextWorkflowStateId;

                history.SetEntityId(entity.Id);

                workflowRepository.AddHistory(history);
                workflowRepository.Save();
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        private bool ValidateAccess(WorkflowStateTransition transition, UserLiteModel currentUser, WorkflowEntity entity)
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
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.Code == appSetting.SectorUserSource))
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

        private bool ValidateManagerAccess(WorkflowStateTransition transition, UserLiteModel currentUser, WorkflowEntity entity, bool hasAccess)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.Code == appSetting.ManagerUserSource))
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

        private bool ValidateApplicantAccess(WorkflowStateTransition transition, UserLiteModel currentUser, WorkflowEntity entity, bool hasAccess)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.Code == appSetting.ApplicantUserSource && entity.UserApplicantId == currentUser.Id))
            {
                hasAccess = true;
            }

            return hasAccess;
        }

        private bool ValidateGroupAccess(WorkflowStateTransition transition, UserLiteModel currentUser, bool hasAccess)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.Code == appSetting.GroupUserSource))
            {
                var user = unitOfWork.UserRepository.GetSingleWithUserGroup(x => x.Id == currentUser.Id);
                var groups = user.UserGroups?.Select(x => x.Group).Distinct().ToList();

                if (groups != null && groups.Any())
                {
                    if (transition.WorkflowStateAccesses.Any(x => groups.Any(u => u.Id == x.UserSource.SourceId) && x.UserSource.Code == appSetting.GroupUserSource))
                    {
                        hasAccess = true;
                    }
                }
            }

            return hasAccess;
        }

        private bool ValidateUserAccess(WorkflowStateTransition transition, UserLiteModel currentUser, bool hasAccess)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.SourceId == currentUser.Id && x.UserSource.Code == appSetting.UserUserSource))
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

        private void AddTransition(Response<IList<TransitionItemModel>> response, WorkflowStateTransition transition)
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
