using Sofco.Core.CrmServices;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusFactory : ISolfacStatusFactory
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ICrmInvoiceService crmInvoiceService;

        private readonly IMailBuilder mailBuilder;

        public SolfacStatusFactory(IUnitOfWork unitOfWork, ICrmInvoiceService crmInvoiceService, IMailBuilder mailBuilder)
        {
            this.unitOfWork = unitOfWork;
            this.crmInvoiceService = crmInvoiceService;
            this.mailBuilder = mailBuilder;
        }

        public ISolfacStatusHandler GetInstance(SolfacStatus status)
        {
            switch (status)
            {
                case SolfacStatus.PendingByManagementControl: return new SolfacStatusPendingByManagementControlHandler(unitOfWork, crmInvoiceService, mailBuilder);
                case SolfacStatus.ManagementControlRejected: return new SolfacStatusManagementControlRejectedHandler(unitOfWork, crmInvoiceService, mailBuilder);
                case SolfacStatus.InvoicePending: return new SolfacStatusInvoicePendingHandler(unitOfWork, crmInvoiceService, mailBuilder);
                case SolfacStatus.Invoiced: return new SolfacStatusInvoicedHandler(unitOfWork, crmInvoiceService, mailBuilder);
                case SolfacStatus.AmountCashed: return new SolfacStatusAmountCashedHandler(unitOfWork, crmInvoiceService, mailBuilder);
                case SolfacStatus.RejectedByDaf: return new SolfacStatusRejectedByDafHandler(unitOfWork, mailBuilder);
                default: return null;
            }
        }
    }
}
