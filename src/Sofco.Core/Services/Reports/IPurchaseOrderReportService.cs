using System.Collections.Generic;
using Sofco.Core.Models.Reports;
using Sofco.Domain.DTO;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Reports
{
    public interface IPurchaseOrderReportService
    {
        Response<List<PurchaseOrderBalanceViewModel>> Get(SearchPurchaseOrderParams parameters);

        Response<List<Option>> GetAnalyticsByCurrentUser();

        Response<List<PurchaseOrderBalanceViewModel>> GetActives(SearchPurchaseOrderParams parameters);
    }
}