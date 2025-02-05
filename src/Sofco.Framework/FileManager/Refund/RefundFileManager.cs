﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Sofco.Core.FileManager;
using Sofco.Core.Models.AdvancementAndRefund.Common;
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

        public ExcelPackage CreateExcel(Domain.Models.AdvancementAndRefund.Refund refund, Employee employee, AdvancementRefundModel resume)
        {
            var memoryStream = this.GetTemplateStream().Result;

            var excel = new ExcelPackage(memoryStream);

            return Create(excel, refund, employee, resume);
        }

        private ExcelPackage Create(ExcelPackage excel, Domain.Models.AdvancementAndRefund.Refund refund, Employee employee, AdvancementRefundModel resume)
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
                var date = refundDetail.CreationDate;
                sheet.Cells[$"A{index}"].Value = $"{date.Day}/{date.Month}/{date.Year}";
                sheet.Cells[$"B{index}"].Value = refundDetail.Description;
                sheet.Cells[$"C{index}"].Value = refundDetail.CostType?.Text;
                sheet.Cells[$"D{index}"].Value = refundDetail.Ammount;

                index++;
                sheet.InsertRow(index, 1);
            }

            sheet.DeleteRow(index, 1);
            index += 4;

            foreach (var refundHistory in refund.Histories)
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

                sheet.Cells[$"D3"].Value = totalAdvancements;
                sheet.Cells[$"D4"].Value = totalRefunds;

                if (totalAdvancements < totalRefunds)
                {
                    sheet.Cells[$"D5"].Value = 0;
                    sheet.Cells[$"D6"].Value = totalRefunds - totalAdvancements;
                }
                else
                {
                    if (totalRefunds < totalAdvancements)
                    {
                        sheet.Cells[$"D5"].Value = totalAdvancements - totalRefunds;
                        sheet.Cells[$"D6"].Value = 0;
                    }
                }
            }
            else
            {
                sheet.Cells[$"D3"].Value = 0;
                sheet.Cells[$"D4"].Value = refund.TotalAmmount;
                sheet.Cells[$"D5"].Value = refund.TotalAmmount;
                sheet.Cells[$"D6"].Value = 0;
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
