using Sofco.Core.Mail;
using Sofco.Resources.Mails;

namespace Sofco.Framework.MailData
{
    public class HitosWithoutSolfac : IMailData
    {
        public MailType MailType => MailType.HitosWithoutSolfac;

        public string Title => MailSubjectResource.HitosWithoutSolfac;

        public string Recipients { get; set; }

        public string Content { get; set; }
    }
}
