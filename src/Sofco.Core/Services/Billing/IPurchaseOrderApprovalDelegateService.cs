﻿using System.Collections.Generic;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
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