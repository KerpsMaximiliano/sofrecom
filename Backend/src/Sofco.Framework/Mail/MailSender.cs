using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sofco.Framework.Mail
{
    public class MailSender
    {
        public static void Send()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Diego", "domiguel@sofrecom.com.ar"));
            message.To.Add(new MailboxAddress("Lucas", "domiguel84@gmail.com"));
            message.Subject = "Hello World - A mail from ASPNET Core";
            message.Body = new TextPart("plain")
            {
                Text = "Hello World - A mail from ASPNET Core"
            };

            using (var client = new SmtpClient())
            {
                //client.Connect("smtp.gmail.com", 587, false);
                client.Connect("sistarSMTP.sofrecom.local", 587, false);

                
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                //client.Authenticate("prudentialpea@gmail.com", "prudentialpeaprudentialpea");
                // Note: since we don't have an OAuth2 token, disable 	// the XOAUTH2 authentication mechanism.     client.Authenticate("anuraj.p@example.com", "password");
                client.Send(message);
                client.Disconnect(true);

                //client.LocalDomain = "some.domain.com";
                //await client.ConnectAsync("smtp.relay.uri", 25, SecureSocketOptions.None).ConfigureAwait(false);
                //await client.SendAsync(emailMessage).ConfigureAwait(false);
                //await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}
