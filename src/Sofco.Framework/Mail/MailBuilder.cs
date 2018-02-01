using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Mail;
using Sofco.Model;
using Sofco.Resources.Mails;

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
                { MailType.Default, MailResource.Default },
                { MailType.HitosWithoutSolfac, MailResource.HitosWithoutSolfac },
                { MailType.EmployeeEndConfirmation, MailResource.EmployeeEndConfirmation }
            };
        }

        private Email GetEmail(MailType mailType, string recipients, string subject, Dictionary<string, string> mailContents)
        {
            var template = MailResource.Template.Replace("{content}", templatesDicts[mailType]);

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

        public Email GetEmail(IMailData mailData)
        {
            var mailContents = mailData.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop =>
                {
                    var data = prop.GetValue(mailData, null);

                    return data == null ? string.Empty : data.ToString();
                });

            return GetEmail(mailData.MailType, mailData.Recipients, mailData.Title, mailContents);
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
            var keyTxt = ToLowerFirst(key);

            return text.Replace("{" + keyTxt + "}", value);
        }

        private static string ToLowerFirst(string source)
        {
            if (string.IsNullOrEmpty(source) || char.IsLower(source, 0))
                return source;

            return char.ToLowerInvariant(source[0]) + source.Substring(1);
        }

        internal static string ReplaceKeyValue(this string text, KeyValuePair<string, string> item)
        {
            return text.ReplaceKeyValue(item.Key, item.Value);
        }
    }
}
