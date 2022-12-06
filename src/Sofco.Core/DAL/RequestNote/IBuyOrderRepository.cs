using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Models.BuyOrder;
using Sofco.Core.Models.RequestNote;

namespace Sofco.Core.DAL.RequestNote
{
    public interface IBuyOrderRepository : IBaseRepository<Sofco.Domain.Models.RequestNote.BuyOrder>
    {
        Domain.Models.RequestNote.BuyOrder GetById(int id);
        IList<Domain.Models.RequestNote.BuyOrder> GetAll(BuyOrderGridFilters filters);
        void UpdateBuyOrder(Domain.Models.RequestNote.BuyOrder buyOrder);
        void InsertBuyOrder(Domain.Models.RequestNote.BuyOrder buyOrder);
        void Save();
    }
}
