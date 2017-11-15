using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Mail;
using Sofco.Model;
using Sofco.Resources;

namespace Sofco.Framework.Mail
{
    public class MailBuilder : IMailBuilder
    {
        private readonly Dictionary<MailType, string> templatesDicts;

        private readonly EmailConfig emailConfig;

        public MailBuilder(IOptions<EmailConfig> emailConfigOptions)
        {
            emailConfig = emailConfigOptions.Value;


            templatesDicts = new Dictionary<MailType, string>
            {
                { MailType.Default, MailResource.DefaultTemplate }
            };
        }

        public Email GetEmail(MailType mailType, string recipients, string subject, Dictionary<string, string> mailContents)
        {
            var template = templatesDicts[mailType];

            var body = template;

            foreach (var item in mailContents)
            {
                body = body.ReplaceKeyValue(item);
            }

            body = body.ReplaceKeyValue(MailContentKey.HomeLink, emailConfig.SiteUrl);

            return new Email
            {
                Subject = subject,
                Recipient = recipients,
                Body = body
            };
        }

        public Email GetSupportEmail(string subject, string message)
        {
            var data = new Dictionary<string, string>
            {
                {MailContentKey.Title, subject},
                {MailContentKey.Message, message},
                {MailContentKey.Content, string.Empty}
            };

            var recipients = emailConfig.SupportMailTo;

            return GetEmail(MailType.Default, recipients, subject, data);

        }

        public Email GetSupportEmail(string subject, Dictionary<string, string> mailContents)
        {
            var template = templatesDicts[MailType.Default];

            var body = template;

            foreach (var item in mailContents)
            {
                body = body.ReplaceKeyValue(item);
            }

            body = body.ReplaceKeyValue(MailContentKey.HomeLink, emailConfig.SiteUrl);

            return new Email
            {
                Subject = subject,
                Recipient = emailConfig.EmailFrom,
                Body = body
            };
        }
    }

    internal static class MailBuilderExtensions
    {
        internal static string ReplaceKeyValue(this string text, string key, string value)
        {
            return text.Replace("{" + key + "}", value);
        }

        internal static string ReplaceKeyValue(this string text, KeyValuePair<string, string> item)
        {
            return text.ReplaceKeyValue(item.Key, item.Value);
        }
    }
}
