using System.Collections.Generic;
using Sofco.Core.Mail;
using Sofco.Resources.Mails;

namespace Sofco.Framework.MailData
{
    public class EmployeeEndNotificationData : IMailData
    {
        public MailType MailType => MailType.Default;

        public string Title => MailSubjectResource.EmployeeEndNotification;

        public string Recipient { get; set; }

        public List<string> Recipients { get; set; }

        public string Message { get; set; }
    }
}
