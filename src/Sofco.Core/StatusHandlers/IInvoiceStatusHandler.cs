using Sofco.Core.Config;
using Sofco.Model.DTO;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.StatusHandlers
{
    public interface IInvoiceStatusHandler
    {
        Response Validate(Invoice invoice, InvoiceStatusParams parameters);
        string GetBodyMail(Invoice invoice, string siteUrl);
        string GetSubjectMail(Invoice invoice);
        string GetSuccessMessage();
        string GetRecipients(Invoice invoice, EmailConfig emailConfig);
        void SaveStatus(Invoice invoice, InvoiceStatusParams parameters);
    }
}
