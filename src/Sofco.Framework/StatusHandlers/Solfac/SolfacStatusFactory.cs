using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Domain.Enums;
using Sofco.Service.Crm.Interfaces;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusFactory : ISolfacStatusFactory
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ICrmInvoicingMilestoneService crmInvoiceService;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        private readonly AppSetting appSetting;

        public SolfacStatusFactory(IUnitOfWork unitOfWork, 
            ICrmInvoicingMilestoneService crmInvoiceService, 
            IOptions<AppSetting> appSettingOptions,
            IMailBuilder mailBuilder, 
            IMailSender mailSender)
        {
            this.unitOfWork = unitOfWork;
            this.appSetting = appSettingOptions.Value;
            this.crmInvoiceService = crmInvoiceService;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
        }

        public ISolfacStatusHandler GetInstance(SolfacStatus status)
        {
            switch (status)
            {
                case SolfacStatus.PendingByManagementControl: return new SolfacStatusPendingByManagementControlHandler(unitOfWork, crmInvoiceService, mailBuilder, mailSender);
                case SolfacStatus.ManagementControlRejected: return new SolfacStatusManagementControlRejectedHandler(unitOfWork, crmInvoiceService, mailBuilder, mailSender);
                case SolfacStatus.InvoicePending: return new SolfacStatusInvoicePendingHandler(unitOfWork, crmInvoiceService, mailBuilder, mailSender);
                case SolfacStatus.Invoiced: return new SolfacStatusInvoicedHandler(unitOfWork, crmInvoiceService, mailBuilder, appSetting, mailSender);
                case SolfacStatus.AmountCashed: return new SolfacStatusAmountCashedHandler(unitOfWork, crmInvoiceService, mailBuilder, mailSender);
                case SolfacStatus.RejectedByDaf: return new SolfacStatusRejectedByDafHandler(unitOfWork, mailBuilder, mailSender, crmInvoiceService);
                default: return null;
            }
        }
    }
}
