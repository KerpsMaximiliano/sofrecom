using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Sofco.Core.FileManager;
using Sofco.Domain.Models.Reports;

namespace Sofco.Framework.FileManager.AllocationManagement
{
    public class EmployeeFileManager : IEmployeeFileManager
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public EmployeeFileManager(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public ExcelPackage CreateReport(IList<EmployeeView> list)
        {
            var memoryStream = this.GetTemplateStream().Result;

            var excel = new ExcelPackage(memoryStream);

            return Create(excel, list);
        }

        private ExcelPackage Create(ExcelPackage excel, IList<EmployeeView> list)
        {
            var sheet = excel.Workbook.Worksheets.First();

            var i = 2;

            foreach (var employeeView in list)
            {
                sheet.Cells[$"C{i}"].Value = employeeView.EmployeeNumber;

                i++;
            }

            return excel;
        }

        private Task<MemoryStream> GetTemplateStream()
        {
            var fileRoot = new FileInfo($"{hostingEnvironment.ContentRootPath}/wwwroot/excelTemplates/reporte-empleados.xlsx");

            var template = new ExcelPackage(fileRoot, false);

            var memoryStream = new MemoryStream(template.GetAsByteArray());

            template.Dispose();

            return Task.FromResult(memoryStream);
        }
    }
}
