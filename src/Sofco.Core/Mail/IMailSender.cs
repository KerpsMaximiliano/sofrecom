﻿using System.Collections.Generic;
using Sofco.Domain;

namespace Sofco.Core.Mail
{
    public interface IMailSender
    {
        void Send(string recipients, 
            string subject, 
            string body);

        void Send(List<Email> emails);

        void Send(Email email);

        void Send(IMailData mailData);

        void Send(List<IMailData> mailDataList);
    }
}
