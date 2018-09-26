using System.Collections.Generic;
using Sofco.Core.Mail;

namespace Sofco.Framework.MailData
{
    public class SolfacStatusData : IMailData
    {
        public MailType MailType => MailType.Default;

        public string Title { get; set; }

        public string Recipient { get; set; }

        public List<string> Recipients { get; set; }

        public string Message { get; set; }
    }
}
