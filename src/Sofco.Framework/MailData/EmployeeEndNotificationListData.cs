using Sofco.Core.Mail;
using Sofco.Resources.Mails;

namespace Sofco.Framework.MailData
{
    public class EmployeeEndNotificationListData : IMailData
    {
        public MailType MailType => MailType.Default;

        public string Title => MailSubjectResource.EmployeeEndConfirmation;

        public string Recipients { get; set; }

        public string Message { get; set; }
    }
}
