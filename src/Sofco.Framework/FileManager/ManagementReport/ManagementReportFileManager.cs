using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Core.Models.ManagementReport;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sofco.Framework.FileManager.ManagementReport
{
    public class ManagementReportFileManager : IManagementReportFileManager
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IUnitOfWork unitOfWork;

        public ManagementReportFileManager(IHostingEnvironment hostingEnvironment, IUnitOfWork unitOfWork)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.unitOfWork = unitOfWork;
        }

        public ExcelPackage CreateTracingExcel(TracingModel tracing)
        {
            var memoryStream = this.GetTemplateStream().Result;

            var excel = new ExcelPackage(memoryStream);

            return Create(excel, tracing);
        }

        private ExcelPackage Create(ExcelPackage excel, TracingModel tracing)
        {
            var sheet = excel.Workbook.Worksheets.First();

            sheet.Cells[$"B1"].Value = tracing.AnalyticName;

            for (int i = 2; i < tracing.MonthsTracking.Count + 2; i++)
            {
                var item = tracing.MonthsTracking[i - 2];

                sheet.Cells[2, i].Value = item.Display;
                sheet.Cells[3, i].Value = float.Parse((string.IsNullOrEmpty(item.PercentageExpectedTotal)) ? "0" : item.PercentageExpectedTotal, CultureInfo.InvariantCulture.NumberFormat);
                sheet.Cells[4, i].Value = float.Parse((string.IsNullOrEmpty(item.PercentageToEnd)) ? "0" : item.PercentageToEnd, CultureInfo.InvariantCulture.NumberFormat);           
            }

            return excel;
        }

        private Task<MemoryStream> GetTemplateStream()
        {
            var fileRoot = new FileInfo($"{hostingEnvironment.ContentRootPath}/wwwroot/excelTemplates/tracing-template.xlsx");

            var template = new ExcelPackage(fileRoot, false);

            var memoryStream = new MemoryStream(template.GetAsByteArray());

            template.Dispose();

            return Task.FromResult(memoryStream);
        }
    }
}
