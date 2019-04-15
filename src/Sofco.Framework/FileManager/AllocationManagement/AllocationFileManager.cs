using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Sofco.Core.FileManager;

namespace Sofco.Framework.FileManager.AllocationManagement
{
    public class AllocationFileManager : IAllocationFileManager
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public AllocationFileManager(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public ExcelPackage CreateReport(IList<Tuple<string, string, decimal>> list)
        {
            var memoryStream = this.GetTemplateStream().Result;

            var excel = new ExcelPackage(memoryStream);

            return Create(excel, list);
        }

        private ExcelPackage Create(ExcelPackage excel, IList<Tuple<string, string, decimal>> list)
        {
            var sheet = excel.Workbook.Worksheets.First();

            for (int i = 2; i < list.Count + 2; i++)
            {
                var item = list[i - 2];

                sheet.Cells[$"A{i}"].Value = item.Item1;
                sheet.Cells[$"B{i}"].Value = item.Item2;
                sheet.Cells[$"C{i}"].Value = item.Item3;
            }

            return excel;
        }

        private Task<MemoryStream> GetTemplateStream()
        {
            var fileRoot = new FileInfo($"{hostingEnvironment.ContentRootPath}/wwwroot/excelTemplates/asignaciones-masivas.xlsx");

            var template = new ExcelPackage(fileRoot, false);

            var memoryStream = new MemoryStream(template.GetAsByteArray());

            template.Dispose();

            return Task.FromResult(memoryStream);
        }
    }
}
