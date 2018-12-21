using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Common;

namespace Sofco.Core.DAL.AdvancementAndRefund
{
    public interface IRefundRepository : IBaseRepository<Refund>
    {
        List<Refund> GetByParameters(RefundListParameterModel model);

        void InsertFile(RefundFile refundFile);

        Refund GetFullById(int id);

        RefundFile GetFile(int id, int fileId);

        void DeleteFile(RefundFile file);

        IList<RefundHistory> GetHistories(int id);

        bool Exist(int id);

        Refund GetById(int id);
    }
}
