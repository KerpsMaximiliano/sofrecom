using Sofco.Domain.Models.Billing;

namespace Sofco.Core.Managers
{
    public interface IPurchaseOrderStatusRecipientManager
    {
        string GetRecipientsCompliance();

        string GetRecipientsCommercial(PurchaseOrder purchaseOrder);

        string GetRejectCompliance();

        string GetRecipientsOperation(PurchaseOrder purchaseOrder);

        string GetRejectCommercial(PurchaseOrder purchaseOrder);

        string GetRecipientsDaf();

        string GetRejectOperation(PurchaseOrder purchaseOrder);

        string GetRecipientsFinalApproval(PurchaseOrder purchaseOrder);

        string GetRejectDaf(PurchaseOrder purchaseOrder);
    }
}