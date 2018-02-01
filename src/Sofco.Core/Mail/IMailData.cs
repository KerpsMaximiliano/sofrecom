namespace Sofco.Core.Mail
{
    public interface IMailData
    {
        MailType MailType { get; }

        string Title { get; }

        string Recipients { get; set; }
    }
}