using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Models.Admin;
using Sofco.Domain.Enums;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Workflow;

namespace Sofco.Service.Implementations.Workflow
{
    public class WorkflowHelper
    {
        public static void CheckEspecialUsers(WorkflowEntity entity, int actualStateId, int nextStateId, AppSetting appSetting)
        {
            if (appSetting.GeneralDirectorUserId == entity.UserApplicantId)
            {
                if (entity is Refund refund)
                {
                    if (actualStateId == appSetting.WorkflowStatusPendingDirectorId &&
                        nextStateId == appSetting.WorkflowStatusPendingGeneralDirectorId)
                    {
                        entity.StatusId = appSetting.WorkflowStatusDafId;
                    }
                }

                if (entity is Advancement advancement)
                {
                    if (advancement.Type == AdvancementType.Viaticum &&
                        actualStateId == appSetting.WorkflowStatusDraft &&
                        nextStateId == appSetting.WorkflowStatusPendingManagerId)
                    {
                        entity.StatusId = appSetting.WorkflowStatusDafId;
                    }
                }
            }

            if (appSetting.FinancialDirectorUserId == entity.UserApplicantId)
            {
                if (entity is Refund refund && nextStateId == appSetting.WorkflowStatusDafId)
                {
                    entity.StatusId = appSetting.WorkflowStatusGafId;
                }

                if (entity is Advancement advancement)
                {
                    if (advancement.Type == AdvancementType.Viaticum &&
                        actualStateId == appSetting.WorkflowStatusPendingManagerId &&
                        nextStateId == appSetting.WorkflowStatusPendingDirectorId)
                    {
                        entity.StatusId = appSetting.WorkflowStatusGafId;
                    }

                    if (advancement.Type == AdvancementType.Salary &&
                        nextStateId == appSetting.WorkflowStatusDafId)
                    {
                        entity.StatusId = appSetting.WorkflowStatusGafId;
                    }
                }
            }

            if (appSetting.GiselaPerugorriaUserId == entity.UserApplicantId)
            {
                if (entity is Advancement advancement)
                {

                    if (advancement.Type == AdvancementType.Viaticum &&
                        actualStateId == appSetting.WorkflowStatusPendingManagerId &&
                        nextStateId == appSetting.WorkflowStatusPendingDirectorId)
                    {
                        entity.StatusId = appSetting.WorkflowStatusPendingGeneralDirectorId;
                    }

                    if (advancement.Type == AdvancementType.Viaticum && 
                        actualStateId == appSetting.WorkflowStatusPendingManagerId &&
                        nextStateId == appSetting.WorkflowStatusDafId)
                    {
                        entity.StatusId = appSetting.WorkflowStatusGafId;
                    }


                    if (advancement.Type == AdvancementType.Viaticum &&
                        actualStateId == appSetting.WorkflowStatusPendingGeneralDirectorId &&
                        nextStateId == appSetting.WorkflowStatusDafId)
                    {
                        entity.StatusId = appSetting.WorkflowStatusGafId;
                    }

                    if (advancement.Type == AdvancementType.Salary &&
                        actualStateId == appSetting.WorkflowStatusRrhhId &&
                        nextStateId == appSetting.WorkflowStatusDafId)
                    {
                        entity.StatusId = appSetting.WorkflowStatusGafId;
                    }
                }

            }

            if (appSetting.DiegoCegnaUserId == entity.UserApplicantId)
            {
                if (entity is Advancement advancement)
                {
                    if (advancement.Type == AdvancementType.Viaticum &&
                        actualStateId == appSetting.WorkflowStatusPendingManagerId &&
                        nextStateId == appSetting.WorkflowStatusPendingDirectorId)
                    {
                        entity.StatusId = appSetting.WorkflowStatusPendingGeneralDirectorId;
                    }
                }
            }

            if (appSetting.MonicaBimanUserId == entity.UserApplicantId)
            {
                if (entity is Advancement advancement)
                {
                    if (advancement.Type == AdvancementType.Viaticum &&
                        actualStateId == appSetting.WorkflowStatusPendingManagerId &&
                        nextStateId == appSetting.WorkflowStatusPendingDirectorId)
                    {
                        entity.StatusId = appSetting.WorkflowStatusDafId;
                    }
                }
            }
        }

        public static bool ValidatePriviligeAccess<TEntity>(WorkflowStateTransition possibleNextTransition, UserLiteModel user, TEntity entity, IUnitOfWork unitOfWork, AppSetting appSetting) where TEntity : WorkflowEntity
        {
            bool hasAccess = false;

            if (string.IsNullOrWhiteSpace(entity.UsersAlreadyApproved)) return false;

            hasAccess = ValidateManagerAccess(possibleNextTransition, entity, hasAccess, unitOfWork, appSetting, user);

            hasAccess = ValidateAnalyticManagerAccess(possibleNextTransition, entity, hasAccess, unitOfWork, appSetting, user);

            hasAccess = ValidateSectorAccess(possibleNextTransition, entity, hasAccess, unitOfWork, appSetting, user);

            hasAccess = ValidateGroupAccess(possibleNextTransition, entity, hasAccess, unitOfWork, appSetting, user);

            hasAccess = ValidateUserAccess(possibleNextTransition, entity, hasAccess, appSetting, user, unitOfWork);

            return hasAccess;
        }

        private static bool ValidateUserAccess(WorkflowStateTransition transition, WorkflowEntity entity,
            bool hasAccess, AppSetting appSetting, UserLiteModel user, IUnitOfWork unitOfWork)
        {
            var usersAlreadyApprovedSplitted = entity.UsersAlreadyApproved.Split(';');

            foreach (var workflowStateAccess in transition.WorkflowStateAccesses)
            {
                if(workflowStateAccess.UserSource.Code == appSetting.UserUserSource && usersAlreadyApprovedSplitted.Contains(workflowStateAccess.UserSource.SourceId.ToString()) && workflowStateAccess.AccessDenied == false)
                {
                    SetUser(user, workflowStateAccess.UserSource.SourceId, unitOfWork);
                    hasAccess = true;
                }
            }

            return hasAccess;
        }

        private static bool ValidateGroupAccess(WorkflowStateTransition transition, WorkflowEntity entity,
            bool hasAccess, IUnitOfWork unitOfWork, AppSetting appSetting, UserLiteModel user)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.Code == appSetting.GroupUserSource))
            {
                var usersAlreadyApprovedSplitted = entity.UsersAlreadyApproved.Split(';');

                var users = unitOfWork.UserRepository.GetByIdsWithGroups(usersAlreadyApprovedSplitted.Select(x => Convert.ToInt32(x)));

                if (users != null && users.Any())
                {
                    foreach (var user1 in users)
                    {
                        if (transition.WorkflowStateAccesses.Any(x => user1.UserGroups.Any(u => u.GroupId == x.UserSource.SourceId) && x.UserSource.Code == appSetting.GroupUserSource))
                        {
                            SetUser(user, user1.Id, unitOfWork);
                            hasAccess = true;
                        }
                    }
                }
            }

            return hasAccess;
        }

        private static bool ValidateSectorAccess(WorkflowStateTransition transition, WorkflowEntity entity,
            bool hasAccess, IUnitOfWork unitOfWork, AppSetting appSetting, UserLiteModel user)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.Code == appSetting.SectorUserSource))
            {
                var usersAlreadyApprovedSplitted = entity.UsersAlreadyApproved.Split(';');

                if (entity is Refund refund)
                {
                    var director = unitOfWork.AnalyticRepository.GetDirector(refund.AnalyticId);

                    if (director != null)
                    {
                        if (usersAlreadyApprovedSplitted.Contains(director.Id.ToString()))
                        {
                            SetUser(user, director.Id, unitOfWork);
                            hasAccess = true;
                        }

                        if (!hasAccess)
                        {
                            var userApprovers = unitOfWork.UserApproverRepository.GetByAnalyticAndUserId(director.UserName, refund.AnalyticId, UserApproverType.Refund);

                            foreach (var userApprover in userApprovers)
                            {
                                if (usersAlreadyApprovedSplitted.Contains(userApprover.ApproverUserId.ToString()))
                                {
                                    SetUser(user, userApprover.ApproverUserId, unitOfWork);
                                    hasAccess = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    var employee = unitOfWork.EmployeeRepository.GetByEmail(entity.UserApplicant.Email);

                    var sectors = unitOfWork.EmployeeRepository.GetAnalyticsWithSector(employee.Id);

                    foreach (var sector in sectors)
                    {
                        if (usersAlreadyApprovedSplitted.Contains(sector.ResponsableUserId.ToString()))
                        {
                            SetUser(user, sector.ResponsableUserId, unitOfWork);
                            hasAccess = true;
                        }
                    }
                }
            }

            return hasAccess;
        }

        private static bool ValidateAnalyticManagerAccess(WorkflowStateTransition transition, WorkflowEntity entity,
            bool hasAccess, IUnitOfWork unitOfWork, AppSetting appSetting, UserLiteModel user)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.Code == appSetting.AnalyticManagerUserSource))
            {
                if (entity is Refund refund)
                {
                    var analytic = unitOfWork.AnalyticRepository.GetById(refund.AnalyticId);

                    if (analytic != null)
                    {
                        if (analytic.ManagerId.HasValue)
                        {
                            var usersAlreadyApprovedSplitted = entity.UsersAlreadyApproved.Split(';');

                            if (usersAlreadyApprovedSplitted.Contains(analytic.ManagerId.Value.ToString()))
                            {
                                SetUser(user, analytic.ManagerId.Value, unitOfWork);

                                hasAccess = true;
                            }

                            if (!hasAccess)
                            {
                                var userApprovers = unitOfWork.UserApproverRepository.GetByAnalyticAndUserId(analytic.Manager.UserName, refund.AnalyticId, UserApproverType.Refund);

                                foreach (var userApprover in userApprovers)
                                {
                                    if (usersAlreadyApprovedSplitted.Contains(userApprover.ApproverUserId.ToString()))
                                    {
                                        SetUser(user, userApprover.ApproverUserId, unitOfWork);
                                        hasAccess = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return hasAccess;
        }

        private static bool ValidateManagerAccess(WorkflowStateTransition transition, WorkflowEntity entity,
            bool hasAccess, IUnitOfWork unitOfWork, AppSetting appSetting, UserLiteModel user)
        {
            if (transition.WorkflowStateAccesses.Any(x => x.UserSource.Code == appSetting.ManagerUserSource))
            {
                var usersAlreadyApprovedSplitted = entity.UsersAlreadyApproved.Split(';');

                var employee = unitOfWork.EmployeeRepository.GetByEmail(entity.UserApplicant.Email);

                if (employee.ManagerId.HasValue && employee.Manager != null)
                {
                    if (usersAlreadyApprovedSplitted.Contains(employee.ManagerId.Value.ToString()))
                    {
                        SetUser(user, employee.ManagerId.Value, unitOfWork);

                        hasAccess = true;
                    }
                }
            }

            return hasAccess;
        }

        private static void SetUser(UserLiteModel currentUser, int id, IUnitOfWork unitOfWork)
        {
            var user = unitOfWork.UserRepository.Get(id);

            if (user != null)
            {
                currentUser.Id = user.Id;
                currentUser.Name = user.Name;
                currentUser.UserName = user.UserName;
            }
        }
    }
}
