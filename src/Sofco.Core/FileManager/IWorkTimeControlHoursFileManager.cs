using System.Collections.Generic;
using OfficeOpenXml;
using Sofco.Core.Models.WorkTimeManagement;

namespace Sofco.Core.FileManager
{
    public interface IWorkTimeControlHoursFileManager
    {
        ExcelPackage CreateExcel(IList<WorkTimeControlResourceModel> list);
    }
}
