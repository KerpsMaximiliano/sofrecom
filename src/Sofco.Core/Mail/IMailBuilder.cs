using System.Collections.Generic;
using Sofco.Model;

namespace Sofco.Core.Mail
{
    public interface IMailBuilder
    {
        Email GetEmail(MailType mailType, string recipients, string subject, Dictionary<string, string> mailContents);

        Email GetEmail(IMailData mailData);

        Email GetSupportEmail(string subject, string message);
    }
}