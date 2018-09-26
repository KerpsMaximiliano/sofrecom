using System.Collections.Generic;

namespace Sofco.Core.Mail
{
    public interface IMailData
    {
        MailType MailType { get; }

        string Title { get; }

        string Recipient { get; set; }

        List<string> Recipients { get; set; }
    }
}