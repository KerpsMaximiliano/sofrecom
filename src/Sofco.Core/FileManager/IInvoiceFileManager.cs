using OfficeOpenXml;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.FileManager
{
    public interface IInvoiceFileManager
    {
        ExcelPackage CreateInvoiceExcel(Invoice invoice);
    }
}
