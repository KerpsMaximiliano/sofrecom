using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using Sofco.Core.Config;
using Sofco.Core.Mail;
using Sofco.Framework.Helpers;
using Sofco.Model;

namespace Sofco.Framework.Mail
{
    public class MailSender : IMailSender
    {
        private readonly IHostingEnvironment environment;
        private readonly EmailConfig emailConfig;
        private readonly string fromEmail;
        private readonly string fromDisplayName;
        private readonly string smtpServer;
        private readonly int smtpPort;
        private readonly string smtpDomain;
        private readonly string mailDevFolder;

        public MailSender(IHostingEnvironment environment, IOptions<EmailConfig> emailConfigOption)
        {
            this.environment = environment;
            emailConfig = emailConfigOption.Value;
            fromEmail = emailConfig.EmailFrom;
            fromDisplayName = emailConfig.DisplyNameFrom;
            smtpServer = emailConfig.SmtpServer;
            smtpPort = emailConfig.SmtpPort;
            smtpDomain = emailConfig.SmtpDomain;
            mailDevFolder = emailConfig.MailDevFolder;
        }

        /// <summary>
        /// Envía emails a los destinatarios enviados como parametro
        /// </summary>
        /// <param name="recipients">Lista de destinatarios separados por ';'</param>
        /// <param name="subject">Asunto del mail</param>
        /// <param name="body">cuerpo del mail</param>
        public void Send(string recipients, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromDisplayName, fromEmail));
            AddRecipients(message, recipients);
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };

            message.Body = bodyBuilder.ToMessageBody();

            SendMessage(message);
        }

        private void AddRecipients(MimeMessage message, string recipients)
        {
            string[] recipientsMails = recipients.Split(';');
            foreach(var email in recipientsMails)
            {
                message.To.Add(new MailboxAddress(email, email));
            }
        }

        public void Send(List<Email> emails)
        {
            var messages = new List<MimeMessage>();
            foreach(var email in emails)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromDisplayName, fromEmail));
                AddRecipients(message, email.Recipient);
                message.Subject = email.Subject;
                var bodyBuilder = new BodyBuilder { HtmlBody = email.Body };
                message.Body = bodyBuilder.ToMessageBody();
                messages.Add(message);
            }
            SendMessages(messages);
        }

        public void Send(Email email)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromDisplayName, fromEmail));
            AddRecipients(message, email.Recipient);
            message.Subject = email.Subject;
            var bodyBuilder = new BodyBuilder { HtmlBody = email.Body };
            message.Body = bodyBuilder.ToMessageBody();

            SendMessage(message);
        }

        private void SendMessage(MimeMessage message)
        {
            SendMessages(new List<MimeMessage> { message });
        }

        private void SendMessages(List<MimeMessage> messages)
        {
            if (environment.IsDevelopment() || environment.IsEnvironment("localhost"))
            {
                foreach(var message in messages)
                {
                    message.WriteTo(FileHelper.GenerateMailFileName(mailDevFolder));
                    System.Threading.Thread.Sleep(500);
                }
                return;
            }

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(smtpServer, smtpPort, false);
                client.LocalDomain = smtpDomain;
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                foreach (var message in messages)
                {
                    client.Send(message);
                }

                client.Disconnect(true);
            }
        }
    }
}
