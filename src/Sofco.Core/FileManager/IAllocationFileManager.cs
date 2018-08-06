using System;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml;

namespace Sofco.Core.FileManager
{
    public interface IAllocationFileManager
    {
        ExcelPackage CreateReport(IList<Tuple<string, string, decimal>> list);
    }
}
