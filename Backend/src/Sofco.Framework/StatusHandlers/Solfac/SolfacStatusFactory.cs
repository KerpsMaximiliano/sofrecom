using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public static class SolfacStatusFactory
    {
        public static ISolfacStatusHandler GetInstance(SolfacStatus status)
        {
            switch (status)
            {
                case SolfacStatus.PendingByManagementControl: return new SolfacStatusPendingByManagementControlHandler();
                case SolfacStatus.ManagementControlRejected: return new SolfacStatusManagementControlRejectedHandler();
                case SolfacStatus.InvoicePending: return new SolfacStatusInvoicePendingHandler();
                case SolfacStatus.Invoiced: return new SolfacStatusInvoicedHandler();
                case SolfacStatus.AmountCashed: return new SolfacStatusAmountCashedHandler();
                default: return null;
            }
        }
    }
}
