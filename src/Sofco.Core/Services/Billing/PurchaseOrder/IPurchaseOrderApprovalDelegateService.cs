using System.Collections.Generic;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing.PurchaseOrder
{
    public interface IPurchaseOrderApprovalDelegateService
    {
        Response<List<PurchaseOrderApprovalDelegateModel>> GetAll();

        Response<PurchaseOrderApprovalDelegateModel> Save(PurchaseOrderApprovalDelegateModel userApprovalDelegate);

        Response Delete(int userDeletegateId);

        Response<List<UserSelectListItem>> GetComplianceUsers();

        Response<List<UserSelectListItem>> GetDafUsers();
    }
}