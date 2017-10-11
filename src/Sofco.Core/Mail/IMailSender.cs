using System.Collections.Generic;
using Sofco.Model;

namespace Sofco.Core.Mail
{
    public interface IMailSender
    {
        void Send(string recipients, 
            string subject, 
            string body);

        void Send(List<Email> emails);
    }
}
