using OfficeOpenXml;

namespace Sofco.Core.FileManager
{
    public interface IWorkTimeExportFileManager
    {
        ExcelPackage CreateTemplateExcel(int analyticId, int periodId);
    }
}
