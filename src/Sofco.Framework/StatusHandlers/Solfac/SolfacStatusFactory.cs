using Sofco.Core.CrmServices;
using Sofco.Core.DAL;
using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusFactory : ISolfacStatusFactory
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ICrmInvoiceService crmInvoiceService;

        public SolfacStatusFactory(IUnitOfWork unitOfWork, ICrmInvoiceService crmInvoiceService)
        {
            this.unitOfWork = unitOfWork;
            this.crmInvoiceService = crmInvoiceService;
        }

        public ISolfacStatusHandler GetInstance(SolfacStatus status)
        {
            switch (status)
            {
                case SolfacStatus.PendingByManagementControl: return new SolfacStatusPendingByManagementControlHandler(unitOfWork, crmInvoiceService);
                case SolfacStatus.ManagementControlRejected: return new SolfacStatusManagementControlRejectedHandler(unitOfWork, crmInvoiceService);
                case SolfacStatus.InvoicePending: return new SolfacStatusInvoicePendingHandler(unitOfWork, crmInvoiceService);
                case SolfacStatus.Invoiced: return new SolfacStatusInvoicedHandler(unitOfWork, crmInvoiceService);
                case SolfacStatus.AmountCashed: return new SolfacStatusAmountCashedHandler(unitOfWork, crmInvoiceService);
                case SolfacStatus.RejectedByDaf: return new SolfacStatusRejectedByDafHandler(unitOfWork);
                default: return null;
            }
        }
    }
}
