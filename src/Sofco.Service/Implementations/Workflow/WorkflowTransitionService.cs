using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Models.Workflow;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Workflow
{
    public class WorkflowTransitionService : IWorkflowTransitionService
    {
        private readonly IWorkflowTransitionValidation workflowTransitionValidation;
        private readonly ILogMailer<WorkflowTransitionService> logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly AppSetting appSetting;
        private readonly IUserSourceService userSourceService;
        private readonly IUserData userData;

        public WorkflowTransitionService(IWorkflowTransitionValidation workflowTransitionValidation, 
            IUnitOfWork unitOfWork,
            IUserSourceService userSourceService,
            IUserData userData,
            IOptions<AppSetting> appSettingsOptions,
            ILogMailer<WorkflowTransitionService> logger)
        {
            this.workflowTransitionValidation = workflowTransitionValidation;
            this.logger = logger;
            this.appSetting = appSettingsOptions.Value;
            this.userSourceService = userSourceService;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
        }

        public Response Post(WorkflowTransitionAddModel model)
        {
            var response = new Response();

            workflowTransitionValidation.ValidateAdd(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var currentUser = userData.GetCurrentUser();

                var domain = new WorkflowStateTransition
                {
                    ActualWorkflowStateId = model.ActualWorkflowStateId.GetValueOrDefault(),
                    NextWorkflowStateId = model.NextWorkflowStateId.GetValueOrDefault(),
                    WorkflowId = model.WorkflowId.GetValueOrDefault(),
                    ConditionCode = model.ConditionCode,
                    NotificationCode = model.NotificationCode,
                    ParameterCode = model.ParameterCode,
                    ValidationCode = model.ValidationCode,
                    OnSuccessCode = model.OnSuccessCode,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    CreatedById = currentUser.Id,
                    ModifiedById = currentUser.Id,
                    WorkflowStateAccesses = new List<WorkflowStateAccess>(),
                    WorkflowStateNotifiers = new List<WorkflowStateNotifier>()
                };

                CheckManagerAccess(model, domain);
                CheckUserApplicantAccess(model, domain);
                CheckGroupAccess(model, domain);
                CheckUserAccess(model, domain);
                CheckSectorAccess(model, domain);

                CheckManagerNotifier(model, domain);
                CheckUserApplicantNotifier(model, domain);
                CheckGroupNotifier(model, domain);
                CheckUserNotifier(model, domain);
                CheckSectorNotifier(model, domain);

                unitOfWork.WorkflowRepository.AddTransition(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.Workflow.Workflow.AddTransitionSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Put(WorkflowTransitionAddModel model)
        {
            var response = new Response();

            var transition = unitOfWork.WorkflowRepository.GetTransition(model.Id.GetValueOrDefault());

            if (transition == null)
            {
                response.AddError(Resources.Workflow.Workflow.TransitionNotFound);
                return response;
            }

            workflowTransitionValidation.ValidateAdd(model, response, model.Id.GetValueOrDefault());

            if (response.HasErrors()) return response;

            try
            {
                var currentUser = userData.GetCurrentUser();

                transition.ActualWorkflowStateId = model.ActualWorkflowStateId.GetValueOrDefault();
                transition.NextWorkflowStateId = model.NextWorkflowStateId.GetValueOrDefault();
                transition.WorkflowId = model.WorkflowId.GetValueOrDefault();
                transition.ConditionCode = model.ConditionCode;
                transition.NotificationCode = model.NotificationCode;
                transition.ParameterCode = model.ParameterCode;
                transition.ValidationCode = model.ValidationCode;
                transition.OnSuccessCode = model.OnSuccessCode;
                transition.ModifiedAt = DateTime.UtcNow;
                transition.ModifiedById = currentUser.Id;
                transition.WorkflowStateAccesses = new List<WorkflowStateAccess>();
                transition.WorkflowStateNotifiers = new List<WorkflowStateNotifier>();

                CheckManagerAccess(model, transition);
                CheckUserApplicantAccess(model, transition);
                CheckGroupAccess(model, transition);
                CheckUserAccess(model, transition);
                CheckSectorAccess(model, transition);

                CheckManagerNotifier(model, transition);
                CheckUserApplicantNotifier(model, transition);
                CheckGroupNotifier(model, transition);
                CheckUserNotifier(model, transition);
                CheckSectorNotifier(model, transition);

                unitOfWork.WorkflowRepository.UpdateTransition(transition);
                unitOfWork.Save();

                response.AddSuccess(Resources.Workflow.Workflow.UpdateTransitionSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<WorkflowTransitionAddModel> Get(int id)
        {
            var response = new Response<WorkflowTransitionAddModel>();

            var transition = unitOfWork.WorkflowRepository.GetTransition(id);

            if (transition == null)
            {
                response.AddError(Resources.Workflow.Workflow.TransitionNotFound);
                return response;
            }

            response.Data = new WorkflowTransitionAddModel
            {
                Id = id,
                ActualWorkflowStateId = transition.ActualWorkflowStateId,
                NextWorkflowStateId = transition.NextWorkflowStateId,
                WorkflowId = transition.WorkflowId,
                ValidationCode = transition.ValidationCode,
                ConditionCode = transition.ConditionCode,
                NotificationCode = transition.NotificationCode,
                ParameterCode = transition.ParameterCode,
                OnSuccessCode = transition.OnSuccessCode,
                GroupsHasAccess = new List<int>(),
                UsersHasAccess =  new List<int>(),
                SectorsHasAccess = new List<int>(),
                NotifyToGroups = new List<int>(),
                NotifyToSectors = new List<int>(),
                NotifyToUsers = new List<int>()
            };

            if (transition.WorkflowStateAccesses != null && transition.WorkflowStateAccesses.Any())
            {
                foreach (var workflowStateAccess in transition.WorkflowStateAccesses)
                {
                    if (workflowStateAccess.UserSource.Code == appSetting.ManagerUserSource)
                        response.Data.ManagerHasAccess = true;

                    if (workflowStateAccess.UserSource.Code == appSetting.ApplicantUserSource)
                        response.Data.UserApplicantHasAccess = true;

                    if (workflowStateAccess.UserSource.Code == appSetting.GroupUserSource)
                        response.Data.GroupsHasAccess.Add(workflowStateAccess.UserSource.SourceId);

                    if(workflowStateAccess.UserSource.Code == appSetting.UserUserSource) 
                        response.Data.UsersHasAccess.Add(workflowStateAccess.UserSource.SourceId);

                    if(workflowStateAccess.UserSource.Code == appSetting.SectorUserSource) 
                        response.Data.SectorsHasAccess.Add(workflowStateAccess.UserSource.SourceId);
                }
            }

            if (transition.WorkflowStateNotifiers != null && transition.WorkflowStateNotifiers.Any())
            {
                foreach (var workflowStateNotifier in transition.WorkflowStateNotifiers)
                {
                    if (workflowStateNotifier.UserSource.Code == appSetting.ManagerUserSource)
                        response.Data.NotifyToManager = true;

                    if (workflowStateNotifier.UserSource.Code == appSetting.ApplicantUserSource)
                        response.Data.NotifyToUserApplicant = true;

                    if (workflowStateNotifier.UserSource.Code == appSetting.GroupUserSource)
                        response.Data.NotifyToGroups.Add(workflowStateNotifier.UserSource.SourceId);

                    if (workflowStateNotifier.UserSource.Code == appSetting.UserUserSource)
                        response.Data.NotifyToUsers.Add(workflowStateNotifier.UserSource.SourceId);

                    if (workflowStateNotifier.UserSource.Code == appSetting.SectorUserSource)
                        response.Data.NotifyToSectors.Add(workflowStateNotifier.UserSource.SourceId);
                }
            }

            return response;
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var transition = unitOfWork.WorkflowRepository.GetTransitionLite(id);

            if (transition == null)
            {
                response.AddError(Resources.Workflow.Workflow.TransitionNotFound);
                return response;
            }

            try
            {
                unitOfWork.WorkflowRepository.DeleteTransition(transition);
                unitOfWork.Save();

                response.AddSuccess(Resources.Workflow.Workflow.TransitionDeleted);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void CheckSectorNotifier(WorkflowTransitionAddModel model, WorkflowStateTransition domain)
        {
            if (model.NotifyToSectors != null && model.NotifyToSectors.Any())
            {
                foreach (var sectorId in model.NotifyToSectors)
                {
                    var wfStateNotifier = CreateWorkflowStateNotifier();

                    wfStateNotifier.UserSource = userSourceService.Get(appSetting.SectorUserSource, sectorId);

                    domain.WorkflowStateNotifiers.Add(wfStateNotifier);
                }
            }
        }

        private void CheckUserNotifier(WorkflowTransitionAddModel model, WorkflowStateTransition domain)
        {
            if (model.NotifyToUsers != null && model.NotifyToUsers.Any())
            {
                foreach (var userId in model.NotifyToUsers)
                {
                    var wfStateNotifier = CreateWorkflowStateNotifier();

                    wfStateNotifier.UserSource = userSourceService.Get(appSetting.UserUserSource, userId);

                    domain.WorkflowStateNotifiers.Add(wfStateNotifier);
                }
            }
        }

        private void CheckGroupNotifier(WorkflowTransitionAddModel model, WorkflowStateTransition domain)
        {
            if (model.NotifyToGroups != null && model.NotifyToGroups.Any())
            {
                foreach (var groupId in model.NotifyToGroups)
                {
                    var wfStateNotifier = CreateWorkflowStateNotifier();

                    wfStateNotifier.UserSource = userSourceService.Get(appSetting.GroupUserSource, groupId);

                    domain.WorkflowStateNotifiers.Add(wfStateNotifier);
                }
            }
        }

        private void CheckUserApplicantNotifier(WorkflowTransitionAddModel model, WorkflowStateTransition domain)
        {
            if (model.NotifyToUserApplicant)
            {
                var wfStateNotifier = CreateWorkflowStateNotifier();

                wfStateNotifier.UserSource = userSourceService.Get(appSetting.ApplicantUserSource);

                domain.WorkflowStateNotifiers.Add(wfStateNotifier);
            }
        }

        private void CheckManagerNotifier(WorkflowTransitionAddModel model, WorkflowStateTransition domain)
        {
            if (model.NotifyToManager)
            {
                var wfStateNotifier = CreateWorkflowStateNotifier();

                wfStateNotifier.UserSource = userSourceService.Get(appSetting.ManagerUserSource);

                domain.WorkflowStateNotifiers.Add(wfStateNotifier);
            }
        }

        private void CheckSectorAccess(WorkflowTransitionAddModel model, WorkflowStateTransition domain)
        {
            if (model.SectorsHasAccess != null && model.SectorsHasAccess.Any())
            {
                foreach (var sectorId in model.SectorsHasAccess)
                {
                    var wfStateAccess = CreateWorkflowStateAccess();

                    wfStateAccess.UserSource = userSourceService.Get(appSetting.SectorUserSource, sectorId);

                    domain.WorkflowStateAccesses.Add(wfStateAccess);
                }
            }
        }

        private void CheckUserAccess(WorkflowTransitionAddModel model, WorkflowStateTransition domain)
        {
            if (model.UsersHasAccess != null && model.UsersHasAccess.Any())
            {
                foreach (var userId in model.UsersHasAccess)
                {
                    var wfStateAccess = CreateWorkflowStateAccess();

                    wfStateAccess.UserSource = userSourceService.Get(appSetting.UserUserSource, userId);

                    domain.WorkflowStateAccesses.Add(wfStateAccess);
                }
            }
        }

        private void CheckGroupAccess(WorkflowTransitionAddModel model, WorkflowStateTransition domain)
        {
            if (model.GroupsHasAccess != null && model.GroupsHasAccess.Any())
            {
                foreach (var groupId in model.GroupsHasAccess)
                {
                    var wfStateAccess = CreateWorkflowStateAccess();

                    wfStateAccess.UserSource = userSourceService.Get(appSetting.GroupUserSource, groupId);

                    domain.WorkflowStateAccesses.Add(wfStateAccess);
                }
            }
        }

        private void CheckUserApplicantAccess(WorkflowTransitionAddModel model, WorkflowStateTransition domain)
        {
            if (model.UserApplicantHasAccess)
            {
                var wfStateAccess = CreateWorkflowStateAccess();

                wfStateAccess.UserSource = userSourceService.Get(appSetting.ApplicantUserSource);

                domain.WorkflowStateAccesses.Add(wfStateAccess);
            }
        }

        private void CheckManagerAccess(WorkflowTransitionAddModel model, WorkflowStateTransition domain)
        {
            if (model.ManagerHasAccess)
            {
                var wfStateAccess = CreateWorkflowStateAccess();

                wfStateAccess.UserSource = userSourceService.Get(appSetting.ManagerUserSource);

                domain.WorkflowStateAccesses.Add(wfStateAccess);
            }
        }

        private WorkflowStateAccess CreateWorkflowStateAccess()
        {
            var wfStateAccess = new WorkflowStateAccess();

            wfStateAccess.CreatedAt = DateTime.UtcNow;
            wfStateAccess.CreatedById = userData.GetCurrentUser().Id;
            wfStateAccess.ModifiedAt = DateTime.UtcNow;
            wfStateAccess.ModifiedById = wfStateAccess.CreatedById;
            return wfStateAccess;
        }

        private WorkflowStateNotifier CreateWorkflowStateNotifier()
        {
            var wfStateNotifier = new WorkflowStateNotifier();

            wfStateNotifier.CreatedAt = DateTime.UtcNow;
            wfStateNotifier.CreatedById = userData.GetCurrentUser().Id;
            wfStateNotifier.ModifiedAt = DateTime.UtcNow;
            wfStateNotifier.ModifiedById = wfStateNotifier.CreatedById;

            return wfStateNotifier;
        }
    }
}
