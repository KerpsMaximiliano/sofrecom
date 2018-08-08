using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Sofco.Core.FileManager;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Framework.FileManager.AllocationManagement
{
    public class AnalyticFileManager : IAnalyticFileManager
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public AnalyticFileManager(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public ExcelPackage CreateAnalyticReportExcel(IList<Analytic> analytics)
        {
            var memoryStream = this.GetTemplateStream().Result;

            var excel = new ExcelPackage(memoryStream);

            return Create(excel, analytics);
        }

        private ExcelPackage Create(ExcelPackage excel, IList<Analytic> analytics)
        {
            var sheet = excel.Workbook.Worksheets.First();

            for (int i = 2; i < analytics.Count+2; i++)
            {
                var item = analytics[i-2];

                if (item.CostCenter != null)
                    sheet.Cells[$"A{i}"].Value = item.CostCenter.Description;

                sheet.Cells[$"B{i}"].Value = item.Title;
                sheet.Cells[$"C{i}"].Value = item.Name;
                sheet.Cells[$"D{i}"].Value = item.StartDateContract.ToString("dd/MM/yyyy");
                sheet.Cells[$"E{i}"].Value = item.EndDateContract.ToString("dd/MM/yyyy");
                sheet.Cells[$"F{i}"].Value = GetStatusDescription(item.Status);
                sheet.Cells[$"G{i}"].Value = item.ClientExternalName;
                sheet.Cells[$"H{i}"].Value = item.Service;
                sheet.Cells[$"I{i}"].Value = item.Proposal;

                if (item.Sector != null)
                    sheet.Cells[$"J{i}"].Value = item.Sector.Text;

                if (item.CommercialManager != null)
                    sheet.Cells[$"K{i}"].Value = item.CommercialManager.Name;

                if (item.Manager != null)
                    sheet.Cells[$"L{i}"].Value = item.Manager.Name;

                if (item.Solution != null)
                    sheet.Cells[$"M{i}"].Value = item.Solution.Text;

                if (item.Technology != null)
                    sheet.Cells[$"N{i}"].Value = item.Technology.Text;

                if (item.ServiceType != null)
                    sheet.Cells[$"O{i}"].Value = item.ServiceType.Text;

                if (item.ClientGroup != null)
                    sheet.Cells[$"P{i}"].Value = item.ClientGroup.Text;

                if (item.SoftwareLaw != null)
                    sheet.Cells[$"Q{i}"].Value = item.SoftwareLaw.Text;

                if (item.Activity != null)
                    sheet.Cells[$"R{i}"].Value = item.Activity.Text;
            }

            return excel;
        }

        private Task<MemoryStream> GetTemplateStream()
        {
            var fileRoot = new FileInfo($"{hostingEnvironment.ContentRootPath}/wwwroot/excelTemplates/analytic-report.xlsx");

            var template = new ExcelPackage(fileRoot, false);

            var memoryStream = new MemoryStream(template.GetAsByteArray());

            template.Dispose();

            return Task.FromResult(memoryStream);
        }

        private string GetStatusDescription(AnalyticStatus status)
        {
            switch (status)
            {
                case AnalyticStatus.Open: return "Abierta";
                case AnalyticStatus.Close: return "Cerrada";
                case AnalyticStatus.CloseToExpenses: return "Cerrada para costos";
                default: return string.Empty;
            }
        }
    }
}
