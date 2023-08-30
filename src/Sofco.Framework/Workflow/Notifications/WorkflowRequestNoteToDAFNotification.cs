using System.Linq;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.Models.Workflow;
using Sofco.Domain.Enums;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.RequestNote;
using Sofco.Domain.Models.Workflow;
using Sofco.Resources.Mails;

namespace Sofco.Framework.Workflow.Notifications
{
    public class WorkflowRequestNoteToDAFNotification : WorkflowNotification
    {
        private readonly EmailConfig emailConfig;

        public WorkflowRequestNoteToDAFNotification(IMailSender mailSender, 
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

            var requestNote = (RequestNote)entity;
            if (!this.MustExecute(requestNote))
                return;

            var subject = string.Format(MailSubjectResource.WorkflowNotificationRequestNoteToDAF, requestNote.Id);
            var body = string.Format(MailMessageResource.WorkflowNotificationRequestNoteToDAF,
                requestNote.Id,
                emailConfig.SiteUrl
                );
            this.SendMail(subject, body, transition, entity);
        }

        private bool MustExecute(RequestNote requestNote)
        {
            return requestNote.RequiresEmployeeClient || !requestNote.TravelSection;
        }
    } 
}
