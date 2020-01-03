using System.Collections.Generic;
using Sofco.Core.Models.Admin;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing.PurchaseOrder
{
    public interface IPurchaseOrderApprovalDelegateService
    {
        Response<List<UserSelectListItem>> GetComplianceUsers();

        Response<List<UserSelectListItem>> GetDafUsers();
    }
}