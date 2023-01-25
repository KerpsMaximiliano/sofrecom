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
    public class WorkflowBuyOrderNotificationReject : WorkflowNotification
    {
        private readonly EmailConfig emailConfig;

        public WorkflowBuyOrderNotificationReject(IMailSender mailSender,
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

            var subject = string.Format(MailSubjectResource.WorkflowNotificationBuyOrder, entity.Id);

            var body = string.Format(MailMessageResource.WorkflowNotificationBuyOrderReject,
                $"{emailConfig.SiteUrl}providers/purchase-orders/edit/{entity.Id}",
                transition.ActualWorkflowState.Name,
                transition.NextWorkflowState.Name,
                comment,
                entity.Id);

            this.SendMail(subject, body, transition, entity);
        }
    }
}
