using OfficeOpenXml;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.FileManager
{
    public interface IRefundFileManager
    {
        ExcelPackage CreateExcel(Refund refund, Employee employee);
    }
}
