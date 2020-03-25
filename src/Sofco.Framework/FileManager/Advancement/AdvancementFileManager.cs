using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Sofco.Core.FileManager;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Spire.Pdf;

namespace Sofco.Framework.FileManager.Advancement
{
    public class AdvancementFileManager : IAdvancementFileManager
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public AdvancementFileManager(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public ExcelPackage CreateExcel(Domain.Models.AdvancementAndRefund.Advancement advancement, Employee employee, AdvancementRefundModel resume)
        {
            var memoryStream = this.GetTemplateStream().Result;

            var excel = new ExcelPackage(memoryStream);

            return Create(excel, advancement, employee, resume);
        }

        private ExcelPackage Create(ExcelPackage excel, Domain.Models.AdvancementAndRefund.Advancement advancement, Employee employee, AdvancementRefundModel resume)
        {
            var sheet = excel.Workbook.Worksheets.First();

            sheet.Cells["A1"].Value += $" {advancement.Id.ToString()}";
            sheet.Cells["B3"].Value = advancement.UserApplicant?.Name;
            sheet.Cells["B4"].Value = employee.OfficeAddress;
            sheet.Cells["B5"].Value = employee.Bank;
            sheet.Cells["B7"].Value = advancement.Status?.Name;

            var creationDate = advancement.CreationDate;
            sheet.Cells["B6"].Value = $"{creationDate.Day}/{creationDate.Month}/{creationDate.Year}";

            sheet.Cells["D5"].Value = advancement.Ammount;
            sheet.Cells["D6"].Value = advancement.Currency?.Text;
            sheet.Cells["D8"].Value = advancement.AdvancementReturnForm;
            sheet.Cells["B15"].Value = advancement.Description;

            sheet.Cells["D3"].Value = advancement.Type == AdvancementType.Salary ? "Sueldo" : "Viatico";

            switch (advancement.PaymentForm)
            {
                case AdvancementPaymentForm.Cash: sheet.Cells["D4"].Value = "Efectivo"; break;
                case AdvancementPaymentForm.ForeignCurrency: sheet.Cells["D4"].Value = "Moneda Extranjera"; break;
                case AdvancementPaymentForm.OwnBank: sheet.Cells["D4"].Value = "Cuenta Sueldo"; break;
            }

            if (advancement.Type == AdvancementType.Salary)
            {
                sheet.Cells["C7"].Value = "Mes de Devolución";
                sheet.Cells["D7"].Value = advancement.MonthsReturn.Text;
            }
            else
            {
                sheet.Cells["C7"].Value = "Fecha Aproximada de Devolución";
                var date = advancement.StartDateReturn.GetValueOrDefault().AddHours(-3);
                sheet.Cells["D7"].Value = $"{date.Day}/{date.Month}/{date.Year} {date.Hour}:{date.Minute}";
            }

            var index = 24;

            foreach (var refundHistory in advancement.Histories)
            {
                var date = refundHistory.CreatedDate.AddHours(-3);
                sheet.Cells[$"A{index}"].Value = $"{date.Day}/{date.Month}/{date.Year} {date.Hour}:{date.Minute}";
                sheet.Cells[$"B{index}"].Value = refundHistory.UserName;
                sheet.Cells[$"C{index}"].Value = refundHistory.StatusFrom?.Name;
                sheet.Cells[$"D{index}"].Value = refundHistory.StatusTo?.Name;
                sheet.Cells[$"E{index}"].Value = refundHistory.Comment;

                index++;
                sheet.InsertRow(index, 1);
            }

            if (resume != null)
            {
                index = 3;

                foreach (var refundResume in resume.Refunds)
                {
                    sheet.Cells[$"G{index}"].Value = refundResume.Id;
                    sheet.Cells[$"H{index}"].Value = refundResume.Analytic;
                    sheet.Cells[$"I{index}"].Value = refundResume.Total;
                    sheet.Cells[$"J{index}"].Value = refundResume.StatusName;
                    sheet.Cells[$"K{index}"].Value = refundResume.LastRefund ? "Si" : "No";
                    sheet.Cells[$"L{index}"].Value = refundResume.CashReturn ? "Si" : "No";

                    index++;
                }

                index = 3;

                foreach (var resumeAdvancement in resume.Advancements)
                {
                    sheet.Cells[$"N{index}"].Value = resumeAdvancement.Id;
                    sheet.Cells[$"O{index}"].Value = resumeAdvancement.Total;

                    index++;
                }

                var totalAdvancements = resume.Advancements.Sum(x => x.Total);
                var totalRefunds = resume.Refunds.Sum(x => x.Total);

                sheet.Cells[$"B9"].Value = totalAdvancements;
                sheet.Cells[$"B10"].Value = totalRefunds;

                if (totalAdvancements < totalRefunds)
                {
                    sheet.Cells[$"B12"].Value = 0;
                    sheet.Cells[$"B11"].Value = totalRefunds - totalAdvancements;
                }
                else
                {
                    if (totalRefunds < totalAdvancements)
                    {
                        sheet.Cells[$"B11"].Value = totalAdvancements - totalRefunds;
                        sheet.Cells[$"B12"].Value = 0;
                    }
                }
            }

            return excel;
        }

        private Task<MemoryStream> GetTemplateStream()
        {
            var fileRoot = new FileInfo($"{_hostingEnvironment.ContentRootPath}/wwwroot/excelTemplates/adelanto-template.xlsx");

            var template = new ExcelPackage(fileRoot, false);

            var memoryStream = new MemoryStream(template.GetAsByteArray());

            template.Dispose();

            return Task.FromResult(memoryStream);
        }
    }
}
