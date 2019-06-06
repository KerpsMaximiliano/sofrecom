using OfficeOpenXml;
using Sofco.Core.Models.ManagementReport;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.FileManager
{
    public interface IManagementReportFileManager
    {
        ExcelPackage CreateTracingExcel(TracingModel tracings);
    }
}
