﻿using System.Collections.Generic;
using Sofco.Core.Models.ManagementReport;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.ManagementReport
{
    public interface IManagementReportBillingService
    {
        Response<int> Update(UpdateValueModel model);

        Response<int> UpdateData(UpdateBillingDataModel model);

        Response UpdateQuantityResources(int idBilling, int quantityResources);

        Response<IList<ResourceBillingRequestItem>> AddResources(int idBilling,
            IList<ResourceBillingRequestItem> resources, string hitoId);

        Response<IList<ResourceBillingRequestItem>> GetResources(int idBilling);

        Response ValidateAddResources(int idBilling, IList<ResourceBillingRequestItem> resources);
    }
}
