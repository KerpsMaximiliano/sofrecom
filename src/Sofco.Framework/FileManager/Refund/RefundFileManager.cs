using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Sofco.Core.FileManager;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Framework.FileManager.Refund
{
    public class RefundFileManager : IRefundFileManager
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public RefundFileManager(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public ExcelPackage CreateExcel(Domain.Models.AdvancementAndRefund.Refund refund, Employee employee)
        {
            var memoryStream = this.GetTemplateStream().Result;

            var excel = new ExcelPackage(memoryStream);

            return Create(excel, refund, employee);
        }

        private ExcelPackage Create(ExcelPackage excel, Domain.Models.AdvancementAndRefund.Refund refund, Employee employee)
        {
            var sheet = excel.Workbook.Worksheets.First();

            sheet.Cells["A1"].Value += $" {refund.Id.ToString()}";
            sheet.Cells["B3"].Value = refund.Status?.Name;
            sheet.Cells["B4"].Value = refund.UserApplicant?.Name;
            sheet.Cells["B5"].Value = employee.OfficeAddress;
            sheet.Cells["B6"].Value = employee.Bank;
            sheet.Cells["B13"].Value = refund.TotalAmmount;
            sheet.Cells["D13"].Value = refund.Currency?.Text;

            if (refund.AdvancementRefunds != null && refund.AdvancementRefunds.Any())
            {
                sheet.Cells["B7"].Value = string.Join(" - ", refund.AdvancementRefunds.Select(x => $"#{x.AdvancementId}").ToList());
            }

            sheet.Cells["B8"].Value = refund.Analytic.Title + " - " + refund.Analytic.Name;

            sheet.Cells["B10"].Value = refund.LastRefund ? "Si" : "No";
            sheet.Cells["B12"].Value = refund.CashReturn ? "Si" : "No";

            if (refund.CreditCard != null)
            {
                sheet.Cells["B11"].Value = refund.CreditCard.Text;
            }
            else
            {
                sheet.Cells["B11"].Value = "No";
            }

            var index = 18;

            foreach (var refundDetail in refund.Details)
            {
                sheet.Cells[$"A{index}"].Value = refundDetail.CreationDate.ToString("d");
                sheet.Cells[$"B{index}"].Value = refundDetail.Description;
                sheet.Cells[$"C{index}"].Value = refundDetail.CostType?.Text;
                sheet.Cells[$"D{index}"].Value = refundDetail.Ammount;

                sheet.InsertRow(index, 1);
                index++;
            }

            index += 6;

            foreach (var refundHistory in refund.Histories)
            {
                sheet.Cells[$"A{index}"].Value = refundHistory.CreatedDate.ToString("d");
                sheet.Cells[$"B{index}"].Value = refundHistory.UserName;
                sheet.Cells[$"C{index}"].Value = refundHistory.StatusFrom?.Name;
                sheet.Cells[$"D{index}"].Value = refundHistory.StatusTo?.Name;
                sheet.Cells[$"E{index}"].Value = refundHistory.Comment;

                sheet.InsertRow(index, 1);
                index++;
            }

            return excel;
        }

        private Task<MemoryStream> GetTemplateStream()
        {
            var fileRoot = new FileInfo($"{_hostingEnvironment.ContentRootPath}/wwwroot/excelTemplates/reintegro-template.xlsx");

            var template = new ExcelPackage(fileRoot, false);

            var memoryStream = new MemoryStream(template.GetAsByteArray());

            template.Dispose();

            return Task.FromResult(memoryStream);
        }
    }
}
