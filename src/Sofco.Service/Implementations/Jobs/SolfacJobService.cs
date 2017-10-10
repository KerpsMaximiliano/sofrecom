using Sofco.Common.Domains;
using Sofco.Core.CrmServices;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Mail;
using Sofco.Core.Services.Jobs;

namespace Sofco.Service.Implementations.Jobs
{
    public class SolfacJobService : ISolfacJobService
    {
        private readonly ISolfacRepository solfacRepository;
        private readonly ICrmInvoiceService crmInvoiceService;
        private readonly IMailSender mailSender;

        public SolfacJobService(ISolfacRepository solfacRepository,
            ICrmInvoiceService crmInvoiceService,
            IMailSender mailSender)
        {
            this.solfacRepository = solfacRepository;
            this.crmInvoiceService = crmInvoiceService;
            this.mailSender = mailSender;
        }

        public Result Get()
        {
            var crmHitosResult = crmInvoiceService.Get(30);

            var subject = "Subject 1";
            var body = "Body One";
            var recipients = "frosales@sofrecom.com.ar";

            mailSender.Send(recipients, subject, body);

            return new Result(crmHitosResult);
        }
    }
}
