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
    public class WorkflowRequestNoteNotificationDefault : WorkflowNotification
    {
        private readonly EmailConfig emailConfig;

        public WorkflowRequestNoteNotificationDefault(IMailSender mailSender,
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

            var subject = string.Format(MailSubjectResource.WorkflowNotificationRequestNote, entity.Id);

            var body = string.Format(MailMessageResource.WorkflowNotificationRequestNote,
                $"{emailConfig.SiteUrl}providers/notes/edit/{entity.Id}",
                transition.ActualWorkflowState.Name,
                transition.NextWorkflowState.Name,
                entity.Id);

            this.SendMail(subject, body, transition, entity);
        }
    }
}
