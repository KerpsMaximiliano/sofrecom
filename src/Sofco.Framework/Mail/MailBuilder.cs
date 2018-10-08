using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Mail;
using Sofco.Domain;
using Sofco.Resources.Mails;

namespace Sofco.Framework.Mail
{
    public class MailBuilder : IMailBuilder
    {
        private const string MailDelimiter = ";";

        private readonly Dictionary<MailType, string> templatesDicts;

        private readonly EmailConfig emailConfig;

        private List<string> allowedMails;

        public MailBuilder(IOptions<EmailConfig> emailConfigOptions)
        {
            emailConfig = emailConfigOptions.Value;


            templatesDicts = new Dictionary<MailType, string>
            {
                { MailType.Default, MailResource.Default },
                { MailType.HitosWithoutSolfac, MailResource.HitosWithoutSolfac },
                { MailType.EmployeeEndConfirmation, MailResource.EmployeeEndConfirmation }
            };

            allowedMails = string.IsNullOrWhiteSpace(emailConfig.AllowedMails) 
                ? new List<string>()
                : emailConfig.AllowedMails.Split(MailDelimiter[0]).ToList();
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

            var mails = new List<string>();

            if (!string.IsNullOrEmpty(mailData.Recipient))
            {
                mails.Add(mailData.Recipient);
            }

            if (mailData.Recipients != null)
            {
                mails.AddRange(mailData.Recipients);
            }

            var recipients = string.Join(MailDelimiter, GetAllowedMails(mails.Distinct()));

            return GetEmail(mailData.MailType, recipients, mailData.Title, mailContents);
        }

        public Email GetSupportEmail(string subject, string message)
        {
            var data = new Dictionary<string, string>
            {
                {MailContentKey.Title, subject},
                {MailContentKey.Message, message},
                {MailContentKey.Content, string.Empty}
            };

            var recipients = emailConfig.SupportMailRecipients;

            return GetEmail(MailType.Default, recipients, subject, data);
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
                Subject = GetSubject(subject),
                Recipient = recipients,
                Body = body
            };
        }

        private string GetSubject(string subject)
        {
            return string.IsNullOrEmpty(emailConfig.PrefixMailEnvironment) 
                ? subject 
                : $"{emailConfig.PrefixMailEnvironment} {subject}";
        }

        private List<string> GetAllowedMails(IEnumerable<string> mails)
        {
            return !allowedMails.Any() 
                ? mails.ToList() 
                : mails.Where(mail => allowedMails.Contains(mail)).ToList();
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
