using System.Collections.Generic;
using OfficeOpenXml;
using Sofco.Domain.Models.Reports;

namespace Sofco.Core.FileManager
{
    public interface IEmployeeFileManager
    {
        ExcelPackage CreateReport(IList<EmployeeView> list);
    }
}
