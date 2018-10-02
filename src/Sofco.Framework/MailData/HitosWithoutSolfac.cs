using System.Collections.Generic;
using Sofco.Core.Mail;
using Sofco.Resources.Mails;

namespace Sofco.Framework.MailData
{
    public class HitosWithoutSolfac : IMailData
    {
        public MailType MailType => MailType.HitosWithoutSolfac;

        public string Title => MailSubjectResource.HitosWithoutSolfac;

        public string Recipient { get; set; }

        public List<string> Recipients { get; set; }

        public string Content { get; set; }
    }
}
