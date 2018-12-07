using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Workflow;
using Sofco.Core.Mail;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Workflow;
using Sofco.Framework.MailData;

namespace Sofco.Framework.Workflow.Notifications
{
    public abstract class WorkflowNotification
    {
        private readonly IMailSender mailSender;
        private readonly IWorkflowRepository workflowRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly AppSetting appSetting;

        protected WorkflowNotification(IMailSender mailSender,
            AppSetting appSetting,
            IUnitOfWork unitOfWork,
            IWorkflowRepository workflowRepository)
        {
            this.mailSender = mailSender;
            this.unitOfWork = unitOfWork;
            this.workflowRepository = workflowRepository;
            this.appSetting = appSetting;
        }

        public abstract void Send(WorkflowEntity entity, WorkflowStateTransition transition);

        protected void SendMail(string subject, string body, WorkflowStateTransition transition, WorkflowEntity entity)
        {
            var recipientsList = new List<string>();

            foreach (var stateNotifier in transition.WorkflowStateNotifiers)
            {
                AddUser(recipientsList, stateNotifier);
                AddGroup(recipientsList, stateNotifier);
                AddApplicant(entity, recipientsList, stateNotifier);
                AddManager(entity, recipientsList, stateNotifier);
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

        private void AddManager(WorkflowEntity entity, List<string> recipientsList, WorkflowStateNotifier stateNotifier)
        {
            if (stateNotifier.UserSource.Code == appSetting.ManagerUserSource)
            {
                if (entity.AuthorizerId.HasValue)
                {
                    if (entity.Authorizer != null && !string.IsNullOrWhiteSpace(entity.Authorizer.Email))
                        recipientsList.Add(entity.Authorizer.Email);
                }
                else
                {
                    if (entity.UserApplicant != null)
                    {
                        var employee = unitOfWork.EmployeeRepository.GetByEmail(entity.UserApplicant.Email);

                        if (employee != null && employee.ManagerId.HasValue && employee.Manager != null && !string.IsNullOrWhiteSpace(employee.Manager.Email))
                            recipientsList.Add(employee.Manager.Email);
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
