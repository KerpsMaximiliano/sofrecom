using System.Collections.Generic;
using Sofco.Core.Models.Reports;
using Sofco.Model.DTO;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Reports
{
    public interface IPurchaseOrderReportService
    {
        Response<List<PurchaseOrderBalanceViewModel>> Get(SearchPurchaseOrderParams parameters);

        Response<List<Option>> GetAnalyticsByCurrentUser();
    }
}