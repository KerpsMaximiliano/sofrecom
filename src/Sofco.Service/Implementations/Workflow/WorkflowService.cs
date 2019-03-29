using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Workflow;
using Sofco.Core.Logger;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.Workflow;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Enums;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Workflow;
using Sofco.Domain.Utils;
using Sofco.Framework.Workflow.Notifications;

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
        private readonly IWorkflowValidation workflowValidation;
        private readonly IOnTransitionSuccessFactory onTransitionSuccessFactory;
        private readonly IRefundValidation refundValidation;

        public WorkflowService(IWorkflowRepository workflowRepository,
            ILogMailer<WorkflowService> logger,
            IUserData userData,
            IUnitOfWork unitOfWork,
            IWorkflowValidationStateFactory workflowValidationStateFactory,
            IOptions<AppSetting> appSettingsOptions,
            IWorkflowNotificationFactory workflowNotificationFactory,
            IWorkflowValidation workflowValidation,
            IRefundValidation refundValidation,
            IOnTransitionSuccessFactory onTransitionSuccessFactory,
            IWorkflowConditionStateFactory workflowConditionStateFactory)
        {
            this.workflowRepository = workflowRepository;
            this.logger = logger;
            this.userData = userData;
            this.workflowConditionStateFactory = workflowConditionStateFactory;
            this.workflowValidationStateFactory = workflowValidationStateFactory;
            this.unitOfWork = unitOfWork;
            this.appSetting = appSettingsOptions.Value;
            this.refundValidation = refundValidation;
            this.workflowNotificationFactory = workflowNotificationFactory;
            this.workflowValidation = workflowValidation;
            this.onTransitionSuccessFactory = onTransitionSuccessFactory;
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
                var codes = transition.ValidationCode.Split(';');

                foreach (var code in codes)
                {
                    var validatorHandler = workflowValidationStateFactory.GetInstance(code);

                    validatorHandler?.Validate(entity, response, parameters);
                }

                if(response.HasErrors()) return response;
            }

            // Save change status
            try
            {
                entity.StatusId = parameters.NextStateId;

                workflowRepository.UpdateStatus(entity);
                workflowRepository.Save();

                response.AddSuccess(Resources.Workflow.Workflow.TransitionSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            // Custom Success Process
            if (!string.IsNullOrWhiteSpace(transition.OnSuccessCode))
            {
                var codes = transition.OnSuccessCode.Split(';');

                foreach (var code in codes)
                {
                    try
                    {
                        var onSuccessHandler = onTransitionSuccessFactory.GetInstance(code);

                        onSuccessHandler?.Process(entity, parameters);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e);
                    }
                }
            }

            // Create history
            CreateHistory<TEntity, THistory>(entity, transition, currentUser, parameters);

            // Send Notification
            SendNotification(entity, response, transition, parameters);

            return response;
        }

        private void SendNotification(WorkflowEntity entity, Response response, WorkflowStateTransition transition,
            WorkflowChangeStatusParameters parameters)
        {
            if (!string.IsNullOrWhiteSpace(transition.NotificationCode))
            {
                try
                {
                    var notificationHandler = workflowNotificationFactory.GetInstance(transition.NotificationCode);
                    notificationHandler?.Send(entity, transition, parameters);
                }
                catch (Exception e)
                {
                    response.AddWarning(Resources.Common.ErrorSendMail);
                    logger.LogError(e);
                }
            }
        }

        private void CreateHistory<TEntity, THistory>(TEntity entity, WorkflowStateTransition transition,
            UserLiteModel currentUser, WorkflowChangeStatusParameters parameters)
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

                if (parameters.Parameters != null && parameters.Parameters.ContainsKey("comments"))
                {
                    history.Comment = parameters.Parameters["comments"];
                }

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

            hasAccess = ValidateUserDenyAccess(transition, currentUser, hasAccess);

            return hasAccess;
        }

        private bool ValidateSectorAccess(WorkflowStateTransition transition, UserLiteModel currentUser, WorkflowEntity entity, bool hasAccess)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.Code == appSetting.SectorUserSource))
            {
                if (entity is Refund refund)
                {
                    var director = unitOfWork.AnalyticRepository.GetDirector(refund.AnalyticId);

                    if (director != null)
                    {
                        if(director.Id == currentUser.Id) hasAccess = true;

                        var userApprovers = unitOfWork.UserApproverRepository.GetByAnalyticAndUserId(director.Id, refund.AnalyticId, UserApproverType.Refund);

                        if (userApprovers.Select(x => x.ApproverUserId).Contains(currentUser.Id))
                        {
                            hasAccess = true;
                        }
                    }
                }
                else
                {
                    var employee = unitOfWork.EmployeeRepository.GetByEmail(entity.UserApplicant.Email);

                    var sectors = unitOfWork.EmployeeRepository.GetAnalyticsWithSector(employee.Id);

                    if (sectors.Any(x => x.ResponsableUserId == currentUser.Id))
                    {
                        hasAccess = true;
                    }
                }
            }

            return hasAccess;
        }

        private bool ValidateManagerAccess(WorkflowStateTransition transition, UserLiteModel currentUser, WorkflowEntity entity, bool hasAccess)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.Code == appSetting.ManagerUserSource))
            {
                var employee = unitOfWork.EmployeeRepository.GetByEmail(entity.UserApplicant.Email);

                if (employee.ManagerId.HasValue && employee.Manager != null)
                {
                    if (employee.ManagerId.Value == currentUser.Id)
                    {
                        hasAccess = true;
                    }

                    if (entity is Refund refund)
                    {
                        var userApprovers = unitOfWork.UserApproverRepository.GetByAnalyticAndUserId(employee.ManagerId.Value, refund.AnalyticId, UserApproverType.Refund);

                        if (userApprovers.Select(x => x.ApproverUserId).Contains(currentUser.Id))
                        {
                            hasAccess = true;
                        }
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
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.SourceId == currentUser.Id && x.UserSource.Code == appSetting.UserUserSource && x.AccessDenied == false))
            {
                hasAccess = true;
            }

            return hasAccess;
        }

        private bool ValidateUserDenyAccess(WorkflowStateTransition transition, UserLiteModel currentUser, bool hasAccess)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.SourceId == currentUser.Id && x.UserSource.Code == appSetting.UserUserSource && x.AccessDenied == true))
            {
                hasAccess = false;
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
                    CheckConditionParameterCode(entity, transition);

                    if (!string.IsNullOrWhiteSpace(transition.ConditionCode))
                    {
                        var conditionHandler = workflowConditionStateFactory.GetInstance(transition.ConditionCode);

                        if (conditionHandler != null && conditionHandler.CanDoTransition(entity, response))
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

        private void CheckConditionParameterCode<T>(T entity, WorkflowStateTransition transition) where T : WorkflowEntity
        {
            if (entity is Refund refund)
            {
                if (appSetting.WorkFlowStatePaymentPending == transition.NextWorkflowStateId ||
                    appSetting.WorkflowStatusFinalizedId == transition.NextWorkflowStateId)
                {
                    var domain = unitOfWork.RefundRepository.GetFullById(entity.Id);

                    if (refund.CurrencyId == appSetting.CurrencyPesos)
                    {
                        if (!refundValidation.HasUserRefund(domain))
                        {
                            transition.ParameterCode = appSetting.CashReturnConfirm;
                        }
                        else
                        {
                            transition.ParameterCode = string.Empty;
                        }
                    }
                    else
                    {
                        if (!refundValidation.HasUserRefund(domain))
                        {
                            transition.ParameterCode = appSetting.CashReturnConfirm;
                        }
                        else if (refund.CurrencyExchange > 0)
                        { 
                            transition.ParameterCode = string.Empty;
                        }
                    }
                }
            }
        }

        public Response<IList<WorkflowListItemModel>> GetAll()
        {
            var response = new Response<IList<WorkflowListItemModel>>();

            response.Data = workflowRepository.GetAll().Select(x => new WorkflowListItemModel(x)).ToList();

            return response;
        }

        public Response<WorkflowDetailModel> GetById(int workflowId)
        {
            var response = new Response<WorkflowDetailModel>();

            var workflow = workflowRepository.GetById(workflowId);

            if (workflow == null)
            {
                response.AddError(Resources.Workflow.Workflow.WorkflowNotFound);
                return response;
            }

            response.Data = new WorkflowDetailModel(workflow);

            return response;
        }

        public Response<WorkflowListItemModel> Post(WorkflowAddModel model)
        {
            var response = new Response<WorkflowListItemModel>();

            workflowValidation.ValidateAdd(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var currentUser = userData.GetCurrentUser();

                var domain = model.CreateDomain();
                domain.CreatedById = currentUser.Id;
                domain.ModifiedById = currentUser.Id;

                var version = unitOfWork.WorkflowRepository.GetVersion(domain.WorkflowTypeId);
                domain.Version = version;

                unitOfWork.WorkflowRepository.Add(domain);

                var wfactive = unitOfWork.WorkflowRepository.GetByTypeActive(domain.WorkflowTypeId);

                if (wfactive != null)
                {
                    wfactive.Active = false;
                    unitOfWork.WorkflowRepository.UpdateActive(wfactive);
                }

                unitOfWork.Save();

                domain.ModifiedBy = new User
                {
                    Id = currentUser.Id,
                    Name = currentUser.Name
                };

                response.Data = new WorkflowListItemModel(domain);
                response.AddSuccess(Resources.Workflow.Workflow.AddSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<IList<Option>> GetTypes()
        {
            var types = unitOfWork.WorkflowRepository.GetTypes();

            var response = new Response<IList<Option>>();

            response.Data = types.Select(x => new Option { Id = x.Id, Text = x.Name }).ToList();

            return response;
        }

        public Response<IList<Option>> GetStates()
        {
            var states = unitOfWork.WorkflowRepository.GetStates();

            var response = new Response<IList<Option>>();

            response.Data = states.Select(x => new Option { Id = x.Id, Text = x.Name }).OrderBy(e => e.Text).ToList();

            return response;
        }

        public Response Put(int id, WorkflowAddModel model)
        {
            var response = new Response();

            var domain = unitOfWork.WorkflowRepository.GetById(id);

            if (domain == null)
            {
                response.AddError(Resources.Workflow.Workflow.WorkflowNotFound);
                return response;
            }
       
            workflowValidation.ValidateUpdate(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var currentUser = userData.GetCurrentUser();

                domain.ModifiedById = currentUser.Id;
                model.UpdateDomain(domain);

                unitOfWork.WorkflowRepository.Update(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.Workflow.Workflow.UpdateSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void AddTransition(Response<IList<TransitionItemModel>> response, WorkflowStateTransition transition)
        {
            response.Data.Add(new TransitionItemModel
            {
                WorkflowId = transition.WorkflowId,
                NextStateId = transition.NextWorkflowStateId,
                NextStateDescription = transition.NextWorkflowState.ActionName,
                WorkFlowStateType = transition.NextWorkflowState.Type,
                ParameterCode = transition.ParameterCode
            });
        }
    }
}
