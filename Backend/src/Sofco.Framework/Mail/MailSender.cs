using MailKit.Net.Smtp;
using MimeKit;

namespace Sofco.Framework.Mail
{
    public class MailSender
    {
        /// <summary>
        /// Envía emails a los destinatarios enviados como parametro
        /// </summary>
        /// <param name="recipients">Lista de destinatarios separados por ';'</param>
        /// <param name="subject">Asunto del mail</param>
        /// <param name="body">cuerpo del mail</param>
        public static void Send(string recipients, string fromEmail, string fromDisplayName, string subject, string body, string smtpServer, int smtpPort, string domain)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromDisplayName, fromEmail));
            AddRecipients(message, recipients);
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(smtpServer, smtpPort, false);
                client.LocalDomain = domain;
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Send(message);
                client.Disconnect(true);
            }
        }

        private static void AddRecipients(MimeMessage message, string recipients)
        {
            string[] recipientsMails = recipients.Split(';');
            foreach(var email in recipientsMails)
            {
                message.To.Add(new MailboxAddress(email, email));
            }
        }
    }
}
