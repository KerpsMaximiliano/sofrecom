using OfficeOpenXml;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.FileManager
{
    public interface IInvoiceFileManager
    {
        ExcelPackage CreateInvoiceExcel(Invoice invoice);
    }
}
