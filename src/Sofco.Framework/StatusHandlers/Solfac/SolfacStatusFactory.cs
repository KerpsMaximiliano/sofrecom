using Sofco.Core.DAL;
using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusFactory : ISolfacStatusFactory
    {
        private readonly IUnitOfWork unitOfWork;

        public SolfacStatusFactory(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public ISolfacStatusHandler GetInstance(SolfacStatus status)
        {
            switch (status)
            {
                case SolfacStatus.PendingByManagementControl: return new SolfacStatusPendingByManagementControlHandler(unitOfWork);
                case SolfacStatus.ManagementControlRejected: return new SolfacStatusManagementControlRejectedHandler(unitOfWork);
                case SolfacStatus.InvoicePending: return new SolfacStatusInvoicePendingHandler(unitOfWork);
                case SolfacStatus.Invoiced: return new SolfacStatusInvoicedHandler(unitOfWork);
                case SolfacStatus.AmountCashed: return new SolfacStatusAmountCashedHandler(unitOfWork);
                case SolfacStatus.RejectedByDaf: return new SolfacStatusRejectedByDafHandler(unitOfWork);
                default: return null;
            }
        }
    }
}
