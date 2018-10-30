using System.Collections.Generic;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing.PurchaseOrder
{
    public interface IPurchaseOrderActiveDelegateService
    {
        Response<List<PurchaseOrderActiveDelegateModel>> GetAll();

        Response<UserDelegate> Save(UserDelegate userDelegate);

        Response Delete(int userDeletegateId);
    }
}