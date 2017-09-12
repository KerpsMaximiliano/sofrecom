using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.StatusHandlers
{
    public interface IInvoiceStatusHandler
    {
        Response Validate(Invoice invoice);
        string GetBodyMail(Invoice invoice, string siteUrl);
        string GetSubjectMail(Invoice invoice);
    }
}
