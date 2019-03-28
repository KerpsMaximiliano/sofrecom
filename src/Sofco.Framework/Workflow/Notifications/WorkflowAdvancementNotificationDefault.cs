using System.Linq;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.Models.Workflow;
using Sofco.Domain.Enums;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Workflow;
using Sofco.Resources.Mails;

namespace Sofco.Framework.Workflow.Notifications
{
    public class WorkflowAdvancementNotificationDefault : WorkflowNotification
    {
        private readonly EmailConfig emailConfig;

        public WorkflowAdvancementNotificationDefault(IMailSender mailSender, 
            EmailConfig emailConfig, 
            AppSetting appSetting,
            IUnitOfWork unitOfWork) : base(mailSender, appSetting, unitOfWork)
        {
            this.emailConfig = emailConfig;
        }

        public override void Send(WorkflowEntity entity, WorkflowStateTransition transition,
            WorkflowChangeStatusParameters parameters)
        {
            if(!transition.WorkflowStateNotifiers.Any()) return;
         
            var advancement = (Advancement)entity;

            var subject = string.Format(MailSubjectResource.WorkflowNotificationAdvancement, entity.UserApplicant.Name);

            var body = string.Format(MailMessageResource.WorkflowNotificationAdvancement,
                advancement.Type == AdvancementType.Salary ? "Sueldo" : "Viatico",
                $"{emailConfig.SiteUrl}advancementAndRefund/advancement/{entity.Id}",
                transition.ActualWorkflowState.Name,
                transition.NextWorkflowState.Name);

            this.SendMail(subject, body, transition, entity);
        }
    }
}
