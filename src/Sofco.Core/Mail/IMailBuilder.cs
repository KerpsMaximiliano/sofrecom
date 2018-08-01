using Sofco.Domain;

namespace Sofco.Core.Mail
{
    public interface IMailBuilder
    {
        Email GetEmail(IMailData mailData);

        Email GetSupportEmail(string subject, string message);
    }
}