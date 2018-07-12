﻿using System.Collections.Generic;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IPurchaseOrderDelegateService
    {
        Response<List<PurchaseOrderDelegateModel>> GetAll();

        Response<PurchaseOrderDelegateModel> Save(PurchaseOrderDelegateModel userDelegate);

        Response Delete(int userDeletegateId);

        Response<List<UserSelectListItem>> GetComplianceUsers();

        Response<List<UserSelectListItem>> GetDafUsers();
    }
}