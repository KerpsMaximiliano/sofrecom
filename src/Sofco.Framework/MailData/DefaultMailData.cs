using Sofco.Core.Mail;

namespace Sofco.Framework.MailData
{
    public class DefaultMailData : IMailData
    {
        public MailType MailType => MailType.Default;

        public string Title { get; set; }

        public string Recipients { get; set; }

        public string Message { get; set; }

        public string Content { get; set; }
    }
}
