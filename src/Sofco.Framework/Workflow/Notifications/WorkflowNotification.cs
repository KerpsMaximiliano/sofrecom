﻿using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.Models.Workflow;
using Sofco.Domain.Enums;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Workflow;
using Sofco.Framework.MailData;
using Sofco.Domain.Models.RequestNote;
using Sofco.Core.Models.RequestNote;

namespace Sofco.Framework.Workflow.Notifications
{
    public abstract class WorkflowNotification
    {
        private readonly IMailSender mailSender;
        private readonly IUnitOfWork unitOfWork;
        private readonly AppSetting appSetting;

        protected WorkflowNotification(IMailSender mailSender,
            AppSetting appSetting,
            IUnitOfWork unitOfWork)
        {
            this.mailSender = mailSender;
            this.unitOfWork = unitOfWork;
            this.appSetting = appSetting;
        }

        public abstract void Send(WorkflowEntity entity, WorkflowStateTransition transition,
            WorkflowChangeStatusParameters parameters);

        protected void SendMail(string subject, string body, WorkflowStateTransition transition, WorkflowEntity entity)
        {
            var recipientsList = new List<string>();

            foreach (var stateNotifier in transition.WorkflowStateNotifiers)
            {
                AddUser(recipientsList, stateNotifier);
                AddGroup(recipientsList, stateNotifier);
                AddApplicant(entity, recipientsList, stateNotifier);
                AddManager(entity, recipientsList, stateNotifier);
                AddAnalyticManager(entity, recipientsList, stateNotifier);
                AddSector(entity, recipientsList, stateNotifier);
            }

            var data = new MailDefaultData()
            {
                Title = subject,
                Message = body,
                Recipients = recipientsList.Distinct().ToList()
            };

            mailSender.Send(data);
        }

        private void AddSector(WorkflowEntity entity, List<string> recipientsList, WorkflowStateNotifier stateNotifier)
        {
            if (stateNotifier.UserSource.Code == appSetting.SectorUserSource)
            {
                if (entity.UserApplicant != null)
                {
                    if (entity is Refund refund)
                    {
                        var director = unitOfWork.AnalyticRepository.GetDirector(refund.AnalyticId);

                        if (director != null)
                        {
                            recipientsList.Add(director.Email);

                            var delegations = unitOfWork.DelegationRepository.GetByUserId(director.Id);

                            var userDelegations = delegations.Where(x => x.Type == DelegationType.RefundApprovall && x.AnalyticSourceId == refund.AnalyticId);

                            foreach (var userDelegation in userDelegations)
                            {
                                recipientsList.Add(userDelegation.GrantedUser.Email);
                            }
                        }
                    }
                    else
                    {
                        var employee = unitOfWork.EmployeeRepository.GetByEmail(entity.UserApplicant.Email);

                        var sectors = unitOfWork.EmployeeRepository.GetAnalyticsWithSector(employee.Id);

                        if (sectors.Any())
                        {
                            foreach (var sector in sectors)
                            {
                                if (sector.ResponsableUser != null && !string.IsNullOrWhiteSpace(sector.ResponsableUser.Email))
                                    recipientsList.Add(sector.ResponsableUser.Email);
                            }
                        }
                    }
                }
            }
        }

        private void AddManager(WorkflowEntity entity, List<string> recipientsList, WorkflowStateNotifier stateNotifier)
        {
            if (stateNotifier.UserSource.Code == appSetting.ManagerUserSource)
            {
                if (entity.UserApplicant != null)
                {
                    var employee = unitOfWork.EmployeeRepository.GetByEmail(entity.UserApplicant.Email);

                    if (employee != null && employee.ManagerId.HasValue && employee.Manager != null && !string.IsNullOrWhiteSpace(employee.Manager.Email))
                    {
                        recipientsList.Add(employee.Manager.Email);

                        if (entity is Refund refund) 
                        {
                            var delegations = unitOfWork.DelegationRepository.GetByUserId(employee.Manager.Id);

                            var userDelegations = delegations.Where(x => x.Type == DelegationType.RefundApprovall && x.AnalyticSourceId == refund.AnalyticId);

                            foreach (var userDelegation in userDelegations)
                            {
                                recipientsList.Add(userDelegation.GrantedUser.Email);
                            }
                        }

                        if (entity is Advancement advancement)
                        {
                            var delegations = unitOfWork.DelegationRepository.GetByUserId(employee.ManagerId.Value);

                            var userDelegations = delegations.Where(x => x.Type == DelegationType.Advancement && (x.UserSourceId == null || x.UserSourceId == advancement.UserApplicantId));

                            foreach (var userDelegation in userDelegations)
                            {
                                recipientsList.Add(userDelegation.GrantedUser.Email);
                            }
                        }
                    }
                        
                }
            }
        }

        private void AddAnalyticManager(WorkflowEntity entity, List<string> recipientsList, WorkflowStateNotifier stateNotifier)
        {
            if (stateNotifier.UserSource.Code == appSetting.AnalyticManagerUserSource)
            {
                if (entity.UserApplicant != null)
                {
                    if (entity is Refund refund)
                    {
                        var manager = unitOfWork.AnalyticRepository.GetManager(refund.AnalyticId);

                        if (manager != null)
                        {
                            recipientsList.Add(manager.Email);

                            var delegations = unitOfWork.DelegationRepository.GetByUserId(manager.Id);

                            var userDelegations = delegations.Where(x => x.Type == DelegationType.RefundApprovall && x.AnalyticSourceId == refund.AnalyticId);

                            foreach (var userDelegation in userDelegations)
                            {
                                recipientsList.Add(userDelegation.GrantedUser.Email);
                            }
                        }
                    }

                    if (entity is RequestNote requestNote)
                    {
                        var analytics = unitOfWork.RequestNoteAnalitycRepository.GetByRequestNoteId(entity.Id);
                        if (analytics != null)
                        {
                            foreach (RequestNoteAnalytic analityc in analytics)
                            {
                                var manager = unitOfWork.AnalyticRepository.GetManager(analityc.AnalyticId);

                                if (manager != null)
                                {
                                    recipientsList.Add(manager.Email);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddApplicant(WorkflowEntity entity, List<string> recipientsList, WorkflowStateNotifier stateNotifier)
        {
            if (stateNotifier.UserSource.Code == appSetting.ApplicantUserSource)
            {
                if (entity.UserApplicant != null && !string.IsNullOrWhiteSpace(entity.UserApplicant.Email))
                    recipientsList.Add(entity.UserApplicant.Email);
            }
        }

        private void AddGroup(List<string> recipientsList, WorkflowStateNotifier stateNotifier)
        {
            if (stateNotifier.UserSource.Code == appSetting.GroupUserSource)
            {
                var group = unitOfWork.GroupRepository.Get(stateNotifier.UserSource.SourceId);

                if (group != null && !string.IsNullOrWhiteSpace(group.Email))
                    recipientsList.Add(group.Email);
            }
        }

        private void AddUser(List<string> recipientsList, WorkflowStateNotifier stateNotifier)
        {
            if (stateNotifier.UserSource.Code == appSetting.UserUserSource)
            {
                var user = unitOfWork.UserRepository.Get(stateNotifier.UserSource.SourceId);

                if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                    recipientsList.Add(user.Email);
            }
        }
    }
}
