using Sofco.Core.Models.BuyOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Services.RequestNote
{
    public interface IBuyOrderService
    {
        IList<BuyOrderGridModel> GetAll(BuyOrderGridFilters filters);
    }
}
