using System.Linq;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Workflow;
using Sofco.Core.Mail;
using Sofco.Core.Models.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Workflow;
using Sofco.Resources.Mails;

namespace Sofco.Framework.Workflow.Notifications
{
    public class WorkflowRefundNotificationReject : WorkflowNotification
    {
        private readonly EmailConfig emailConfig;

        public WorkflowRefundNotificationReject(IMailSender mailSender,
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

            var comment = string.Empty;

            if (parameters.Parameters != null && parameters.Parameters.ContainsKey("comments"))
            {
                comment = parameters.Parameters["comments"];
            }

            var subject = string.Format(MailSubjectResource.WorkflowNotificationRefund, entity.UserApplicant.Name);

            var body = string.Format(MailMessageResource.WorkflowNotificationReject,
                $"{emailConfig.SiteUrl}advancementAndRefund/refund/{entity.Id}",
                transition.ActualWorkflowState.Name,
                transition.NextWorkflowState.Name,
                comment);

            this.SendMail(subject, body, transition, entity);
        }
    }
}
