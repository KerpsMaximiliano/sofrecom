using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Core.DAL.AdvancementAndRefund
{
    public interface IRefundRepository : IBaseRepository<Refund>
    {
        void InsertFile(RefundFile refundFile);

        Refund GetFullById(int id);
    }
}
