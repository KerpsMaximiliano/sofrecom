using System.Collections.Generic;
using OfficeOpenXml;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.FileManager
{
    public interface IAnalyticFileManager
    {
        ExcelPackage CreateAnalyticReportExcel(IList<Analytic> analytics);
    }
}
