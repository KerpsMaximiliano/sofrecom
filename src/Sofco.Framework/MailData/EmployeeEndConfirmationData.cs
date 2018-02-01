using Sofco.Core.Mail;
using Sofco.Resources.Mails;

namespace Sofco.Framework.MailData
{
    public class EmployeeEndConfirmationData : IMailData
    {
        public MailType MailType => MailType.EmployeeEndConfirmation;

        public string Title => MailSubjectResource.EmployeeEndConfirmation;

        public string Recipients { get; set; }

        public string EmployeeNumber { get; set; }

        public string Name { get; set; }

        public string EndDate { get; set; }
    }
}
