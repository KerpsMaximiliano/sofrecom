using System.IO;
using OfficeOpenXml;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.FileManager
{
    public interface IAdvancementFileManager
    {
        ExcelPackage CreateExcel(Advancement advancement, Employee employee, AdvancementRefundModel resume);
    }
}
