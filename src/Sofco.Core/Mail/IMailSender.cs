namespace Sofco.Core.Mail
{
    public interface IMailSender
    {
        void Send(string recipients, 
            string subject, 
            string body);
    }
}
