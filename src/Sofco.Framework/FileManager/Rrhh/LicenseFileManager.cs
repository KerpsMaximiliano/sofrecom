using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Sofco.Core.FileManager;
using Sofco.Model.Enums;
using Sofco.Model.Models.Rrhh;

namespace Sofco.Framework.FileManager.Rrhh
{
    public class LicenseFileManager : ILicenseFileManager
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly char[] excelColumns =  { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };

        public LicenseFileManager(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public ExcelPackage CreateLicenseReportExcel(IList<License> licenses)
        {
            var memoryStream = this.GetTemplateStream().Result;

            var excel = new ExcelPackage(memoryStream);

            return Create(excel, licenses);
        }

        private ExcelPackage Create(ExcelPackage excel, IList<License> licenses)
        {
            var sheet = excel.Workbook.Worksheets.First();

            for (int i = 0; i < licenses.Count; i++)
            {
                var index = i + 2;

                sheet.Cells[$"A{index}"].Value = licenses[i].Employee.EmployeeNumber;
                sheet.Cells[$"B{index}"].Value = licenses[i].Employee.Name;
                sheet.Cells[$"C{index}"].Value = licenses[i].Type.Description;
                sheet.Cells[$"D{index}"].Value = licenses[i].StartDate.ToString("dd/MM/yyyy");
                sheet.Cells[$"E{index}"].Value = licenses[i].EndDate.ToString("dd/MM/yyyy");
                sheet.Cells[$"F{index}"].Value = licenses[i].DaysQuantity;
                sheet.Cells[$"G{index}"].Value = GetStatusDescription(licenses[i].Status);
                sheet.Cells[$"H{index}"].Value = licenses[i].Employee.HolidaysPending;
                sheet.Cells[$"I{index}"].Value = licenses[i].Employee.HolidaysByLaw;

                SetBorders(sheet, index);

                index++;
                sheet.InsertRow(index, 1);
            }

            return excel;
        }

        private void SetBorders(ExcelWorksheet sheet, int index)
        {
            foreach (var column in excelColumns)
            {
                sheet.Cells[$"{column}{index}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells[$"{column}{index}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                sheet.Cells[$"{column}{index}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            }
        }

        private Task<MemoryStream> GetTemplateStream()
        {
            var fileRoot = new FileInfo($"{hostingEnvironment.ContentRootPath}/wwwroot/excelTemplates/license-report.xlsx");

            var template = new ExcelPackage(fileRoot, false);

            var memoryStream = new MemoryStream(template.GetAsByteArray());

            template.Dispose();

            return Task.FromResult(memoryStream);
        }

        private string GetStatusDescription(LicenseStatus status)
        {
            switch (status)
            {
                case LicenseStatus.Draft: return "Borrador";
                case LicenseStatus.AuthPending: return "Pendiente autorización";
                case LicenseStatus.Pending: return "Pendiente aprobación";
                case LicenseStatus.ApprovePending: return "Aprobada / Falta documentación";
                case LicenseStatus.Approved: return "Aprobada";
                case LicenseStatus.Rejected: return "Rechazada";
                default: return string.Empty;
            }
        }
    }
}
