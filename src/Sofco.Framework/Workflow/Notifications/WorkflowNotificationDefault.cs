using System;
using System.Collections.Generic;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Workflow;
using Sofco.Core.Mail;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Enums;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Workflow;
using Sofco.Domain.Utils;
using Sofco.Framework.MailData;
using Sofco.Resources.Mails;

namespace Sofco.Framework.Workflow.Notifications
{
    public class WorkflowNotificationDefault : IWorkflowNotification
    {
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;
        private readonly IUnitOfWork unitOfWork;
        private readonly IWorkflowRepository workflowRepository;
        private readonly AppSetting appSetting;

        public WorkflowNotificationDefault(IMailSender mailSender, 
            EmailConfig emailConfig, 
            AppSetting appSetting,
            IUnitOfWork unitOfWork, 
            IWorkflowRepository workflowRepository)
        {
            this.mailSender = mailSender;
            this.emailConfig = emailConfig;
            this.unitOfWork = unitOfWork;
            this.workflowRepository = workflowRepository;
            this.appSetting = appSetting;
        }

        public void Send(WorkflowEntity entity, Response response, WorkflowStateTransition transition)
        {
            try
            {
                var advancement = (Advancement)entity;

                var subject = string.Format(MailSubjectResource.WorkflowNotificationAdvancement, entity.UserApplicant.Name);

                var body = string.Format(MailMessageResource.WorkflowNotificationAdvancement,
                    advancement.Type == AdvancementType.Salary ? "Sueldo" : "Viatico",
                    $"{emailConfig.SiteUrl}advancementAndRefund/advancement/${entity.Id}",
                    transition.ActualWorkflowState.Name,
                    transition.NextWorkflowState.Name);

                var recipientsList = new List<string>();

                var stateNotifiers = workflowRepository.GetNotifiers(transition.ActualWorkflowStateId, transition.NextWorkflowStateId, appSetting.AdvacementWorkflowId);

                var data = new MailDefaultData()
                {
                    Title = subject,
                    Message = body,
                    Recipients = recipientsList
                };

                mailSender.Send(data);
            }
            catch (Exception e)
            {
                response.AddWarning(Resources.Common.ErrorSendMail);
            }
        }
    }
}
