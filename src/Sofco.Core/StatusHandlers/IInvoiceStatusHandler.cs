using Sofco.Core.Config;
using Sofco.Core.Mail;
using Sofco.Model.DTO;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.StatusHandlers
{
    public interface IInvoiceStatusHandler
    {
        Response Validate(Invoice invoice, InvoiceStatusParams parameters);
        string GetSuccessMessage();
        void SaveStatus(Invoice invoice, InvoiceStatusParams parameters);
        void SendMail(IMailSender mailSender, Invoice invoice, EmailConfig emailConfig);
    }
}
