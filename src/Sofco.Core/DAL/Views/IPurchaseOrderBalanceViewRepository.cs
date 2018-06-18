using System.Collections.Generic;
using Sofco.Model.DTO;
using Sofco.Model.Models.Reports;

namespace Sofco.Core.DAL.Views
{
    public interface IPurchaseOrderBalanceViewRepository
    {
        List<PurchaseOrderBalanceView> Search(SearchPurchaseOrderParams parameters);

        List<PurchaseOrderBalanceDetailView> GetByPurchaseOrderIds(List<int> purchaseOrderIds);
    }
}