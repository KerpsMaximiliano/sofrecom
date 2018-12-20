using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Common;

namespace Sofco.Core.DAL.AdvancementAndRefund
{
    public interface IRefundRepository : IBaseRepository<Refund>
    {
        void InsertFile(RefundFile refundFile);

        Refund GetFullById(int id);

        RefundFile GetFile(int id, int fileId);

        void DeleteFile(RefundFile file);

        IList<RefundHistory> GetHistories(int id);
    }
}
