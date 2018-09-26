using System.Collections.Generic;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.Managers
{
    public interface IPurchaseOrderStatusRecipientManager
    {
        List<string> GetRecipientsCompliance();

        List<string> GetRecipientsCommercial(PurchaseOrder purchaseOrder);

        List<string> GetRejectCompliance();

        List<string> GetRecipientsOperation(PurchaseOrder purchaseOrder);

        List<string> GetRejectCommercial(PurchaseOrder purchaseOrder);

        List<string> GetRecipientsDaf();

        List<string> GetRejectOperation(PurchaseOrder purchaseOrder);

        List<string> GetRecipientsFinalApproval(PurchaseOrder purchaseOrder);

        List<string> GetRejectDaf(PurchaseOrder purchaseOrder);
    }
}