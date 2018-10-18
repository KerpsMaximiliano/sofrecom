using System.Collections.Generic;
using OfficeOpenXml;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.FileManager
{
    public interface IPurchaseOrderFileManager
    {
        ExcelPackage CreateReport(IList<PurchaseOrder> purchaseOrders);
    }
}
