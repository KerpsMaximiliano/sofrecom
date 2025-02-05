﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Sofco.Core.FileManager;
using Sofco.Core.Models.WorkTimeManagement;

namespace Sofco.Framework.FileManager.WorkTime
{
    public class WorkTimeControlHoursFileManager : IWorkTimeControlHoursFileManager
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public WorkTimeControlHoursFileManager(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public ExcelPackage CreateExcel(IList<WorkTimeControlResourceModel> list)
        {
            var memoryStream = this.GetTemplateStream().Result;

            var excel = new ExcelPackage(memoryStream);

            return Create(excel, list);
        }

        private ExcelPackage Create(ExcelPackage excel, IList<WorkTimeControlResourceModel> list)
        {
            var sheet1 = excel.Workbook.Worksheets[1];
            var sheet2 = excel.Workbook.Worksheets[2];

            var businessHours = list.Sum(x => x.BusinessHours);
            var draftHours = list.Sum(x => x.DraftHours);
            var sentHours = list.Sum(x => x.SentHours);
            var approvedHours = list.Sum(x => x.ApprovedHours);
            var rejectedHours = list.Sum(x => x.RejectHours);
            var licenseHours = list.Sum(x => x.LicenseHours);

            var total = draftHours + sentHours + approvedHours + rejectedHours + licenseHours;

            sheet1.Cells[$"B1"].Value = businessHours;
            sheet1.Cells[$"D1"].Value = draftHours;
            sheet1.Cells[$"D2"].Value = sentHours;
            sheet1.Cells[$"D3"].Value = approvedHours;

            sheet1.Cells[$"B2"].Value = total;
            sheet1.Cells[$"B3"].Value = list.Sum(x => x.PendingHours);

            var j = 2;
            var count = list.Count + 5;

            for (int i = 6; i <= count; i++)
            {
                var item = list[i - 6];

                sheet1.Cells[$"A{i}"].Value = item.Analytic;
                sheet1.Cells[$"B{i}"].Value = item.EmployeeNumber;
                sheet1.Cells[$"C{i}"].Value = item.EmployeeName;
                sheet1.Cells[$"D{i}"].Value = item.BusinessHours;
                sheet1.Cells[$"E{i}"].Value = item.ApprovedHours;
                sheet1.Cells[$"F{i}"].Value = item.SentHours;
                sheet1.Cells[$"G{i}"].Value = item.DraftHours;
                sheet1.Cells[$"H{i}"].Value = item.PendingHours;

                foreach (var detail in item.Details)
                {
                    sheet2.Cells[$"A{j}"].Value = $"{item.Analytic}-{item.EmployeeNumber}-{item.EmployeeName}";
                    sheet2.Cells[$"B{j}"].Value = detail.Date.ToString("dd/MM/yyyy");
                    sheet2.Cells[$"C{j}"].Value = detail.Reference;
                    sheet2.Cells[$"D{j}"].Value = detail.Comments;
                    sheet2.Cells[$"E{j}"].Value = detail.TaskDescription;
                    sheet2.Cells[$"F{j}"].Value = detail.CategoryDescription;
                    sheet2.Cells[$"G{j}"].Value = detail.RegisteredHours;

                    j++;
                }

                j++;
            }

            return excel;
        }

        private Task<MemoryStream> GetTemplateStream()
        {
            var fileRoot = new FileInfo($"{hostingEnvironment.ContentRootPath}/wwwroot/excelTemplates/worktime-control-hours.xlsx");

            var template = new ExcelPackage(fileRoot, false);

            var memoryStream = new MemoryStream(template.GetAsByteArray());

            template.Dispose();

            return Task.FromResult(memoryStream);
        }
    }
}
