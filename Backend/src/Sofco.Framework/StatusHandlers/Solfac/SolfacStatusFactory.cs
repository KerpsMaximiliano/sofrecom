using Sofco.Core.DAL.Admin;
using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusFactory : ISolfacStatusFactory
    {
        private readonly IGroupRepository _groupRepository;

        public SolfacStatusFactory(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public ISolfacStatusHandler GetInstance(SolfacStatus status)
        {
            switch (status)
            {
                case SolfacStatus.PendingByManagementControl: return new SolfacStatusPendingByManagementControlHandler(_groupRepository);
                case SolfacStatus.ManagementControlRejected: return new SolfacStatusManagementControlRejectedHandler();
                case SolfacStatus.InvoicePending: return new SolfacStatusInvoicePendingHandler(_groupRepository);
                case SolfacStatus.Invoiced: return new SolfacStatusInvoicedHandler();
                case SolfacStatus.AmountCashed: return new SolfacStatusAmountCashedHandler();
                default: return null;
            }
        }
    }
}
