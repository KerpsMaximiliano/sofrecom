using System.Collections.Generic;
using OfficeOpenXml;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Core.FileManager
{
    public interface ILicenseFileManager
    {
        ExcelPackage CreateLicenseReportExcel(IList<License> licenses);
    }
}
