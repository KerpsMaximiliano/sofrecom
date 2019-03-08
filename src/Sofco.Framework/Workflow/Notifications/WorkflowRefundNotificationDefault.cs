using System.Linq;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Workflow;
using Sofco.Core.Mail;
using Sofco.Core.Models.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Workflow;
using Sofco.Resources.Mails;

namespace Sofco.Framework.Workflow.Notifications
{
    public class WorkflowRefundNotificationDefault : WorkflowNotification
    {
        private readonly EmailConfig emailConfig;

        public WorkflowRefundNotificationDefault(IMailSender mailSender,
            EmailConfig emailConfig,
            AppSetting appSetting,
            IUnitOfWork unitOfWork) : base(mailSender, appSetting, unitOfWork)
        {
            this.emailConfig = emailConfig;
        }

        public override void Send(WorkflowEntity entity, WorkflowStateTransition transition,
            WorkflowChangeStatusParameters parameters)
        {
            if (!transition.WorkflowStateNotifiers.Any()) return;

            var subject = string.Format(MailSubjectResource.WorkflowNotificationRefund, entity.UserApplicant.Name);

            var body = string.Format(MailMessageResource.WorkflowNotificationRefund,
                $"{emailConfig.SiteUrl}advancementAndRefund/refund/{entity.Id}",
                transition.ActualWorkflowState.Name,
                transition.NextWorkflowState.Name);

            this.SendMail(subject, body, transition, entity);
        }
    }
}
