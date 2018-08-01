using System.Collections.Generic;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.Reports;

namespace Sofco.Core.DAL.Views
{
    public interface IPurchaseOrderBalanceViewRepository
    {
        List<PurchaseOrderBalanceView> Search(SearchPurchaseOrderParams parameters);

        List<PurchaseOrderBalanceDetailView> GetByPurchaseOrderIds(List<int> purchaseOrderIds);
    }
}