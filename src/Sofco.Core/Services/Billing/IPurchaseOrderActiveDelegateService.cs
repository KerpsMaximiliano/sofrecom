using System.Collections.Generic;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IPurchaseOrderActiveDelegateService
    {
        Response<List<PurchaseOrderActiveDelegateModel>> GetAll();

        Response<UserDelegate> Save(UserDelegate userDelegate);

        Response Delete(int userDeletegateId);
    }
}