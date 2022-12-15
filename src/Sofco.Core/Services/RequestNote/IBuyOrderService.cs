using Sofco.Core.Models.BuyOrder;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Services.RequestNote
{
    public interface IBuyOrderService
    {
        IList<BuyOrderGridModel> GetAll(BuyOrderGridFilters filters);
        Response<BuyOrderModel> GetById(int id);
        Response<string> Add(BuyOrderModel model);

        Response<IList<Option>> GetStates();
    }
}
