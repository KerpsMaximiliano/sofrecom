using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusFactory : ISolfacStatusFactory
    {
        private readonly IGroupRepository groupRepository;
        private readonly ISolfacRepository solfacRepository;

        public SolfacStatusFactory(IGroupRepository groupRepo, ISolfacRepository solfacRepo)
        {
            groupRepository = groupRepo;
            solfacRepository = solfacRepo;
        }

        public ISolfacStatusHandler GetInstance(SolfacStatus status)
        {
            switch (status)
            {
                case SolfacStatus.PendingByManagementControl: return new SolfacStatusPendingByManagementControlHandler(groupRepository, solfacRepository);
                case SolfacStatus.ManagementControlRejected: return new SolfacStatusManagementControlRejectedHandler();
                case SolfacStatus.InvoicePending: return new SolfacStatusInvoicePendingHandler(groupRepository);
                case SolfacStatus.Invoiced: return new SolfacStatusInvoicedHandler(solfacRepository);
                case SolfacStatus.AmountCashed: return new SolfacStatusAmountCashedHandler();
                default: return null;
            }
        }
    }
}
