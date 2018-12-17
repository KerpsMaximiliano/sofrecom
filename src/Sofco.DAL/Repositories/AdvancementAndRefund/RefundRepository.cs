using Sofco.Core.DAL.AdvancementAndRefund;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.DAL.Repositories.AdvancementAndRefund
{
    public class RefundRepository : BaseRepository<Refund>, IRefundRepository
    {
        public RefundRepository(SofcoContext context) : base(context)
        {
        }
    }
}
