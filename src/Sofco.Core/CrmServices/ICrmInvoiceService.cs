using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Domain.Crm;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Core.CrmServices
{
    public interface ICrmInvoiceService
    {
        Result<List<CrmHito>> GetHitosToExpire(int daysToExpire);

        Result<string> CreateHitoBySolfac(Solfac solfac);

        Response UpdateHitos(ICollection<Hito> hitos);

        void UpdateHitoStatus(List<Hito> hitos, HitoStatus hitoStatus);

        void UpdateHitoInvoice(IList<Hito> list, SolfacStatusParams parameters);

        void UpdateHitosStatus(List<string> hitoIds, HitoStatus hitoStatus);

        void UpdateHitosStatusAndPurchaseOrder(List<string> hitoIds, HitoStatus hitoStatus, string purchaseOrderNumber);

        void UpdateHitosStatusAndInvoiceDateAndNumber(List<string> hitoIds, HitoStatus hitoStatus, DateTime invoicingDate, string invoiceCode);

        void UpdateHitosStatusAndBillingDate(List<string> hitoIds, HitoStatus hitoStatus, DateTime billingDate);
    }
}
