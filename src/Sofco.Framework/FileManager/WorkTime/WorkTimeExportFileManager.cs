using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Core.Models.Common;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Framework.FileManager.WorkTime
{
    public class WorkTimeExportFileManager : IWorkTimeExportFileManager
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<WorkTimeExportFileManager> logger;
        private readonly IHostingEnvironment hostingEnvironment;

        private IList<Holiday> Holidays { get; set; }

        public WorkTimeExportFileManager(IUnitOfWork unitOfWork, ILogMailer<WorkTimeExportFileManager> logger, IHostingEnvironment hostingEnvironment)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.hostingEnvironment = hostingEnvironment;
        }
        public ExcelPackage CreateTemplateExcel(int analyticId)
        {
            var memoryStream = this.GetTemplateStream().Result;

            var excel = new ExcelPackage(memoryStream);

            return Create(excel, analyticId);
        }

        private ExcelPackage Create(ExcelPackage excel, int analyticId)
        {
            var sheet1 = excel.Workbook.Worksheets[1];
            var sheet2 = excel.Workbook.Worksheets[2];

            FillCategories(sheet2);
            FillResources(sheet1, analyticId);

            return excel;
        }

        private void FillResources(ExcelWorksheet sheet1, int analyticId)
        {
            var closeDates = unitOfWork.CloseDateRepository.GetBeforeCurrentAndNext();
            var period = closeDates.GetPeriodIncludeDays();
          
            FillHolidays(period);

            var employees = unitOfWork.AnalyticRepository.GetResources(analyticId);

            var index = 2;

            foreach (var employee in employees)
            {
                var startDate = period.Item1;
                var endDate = period.Item2;
                
                while (startDate.Date <= endDate.Date)
                {
                    if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday &&
                        Holidays.All(x => x.Date.Date != startDate.Date))
                    {
                        sheet1.Cells[$"A{index}"].Value = employee.EmployeeNumber;
                        sheet1.Cells[$"B{index}"].Value = employee.Name;
                        sheet1.Cells[$"C{index}"].Value = startDate.ToString("d");

                        index++;
                    }

                    startDate = startDate.AddDays(1);
                }
            }
        }

        private void FillCategories(ExcelWorksheet sheet2)
        {
            var tasks = unitOfWork.TaskRepository.GetAllActivesWithCategories();

            for (int i = 0; i < tasks.Count; i++)
            {
                var index = i + 2;

                sheet2.Cells[$"A{index}"].Value = tasks[i].Category?.Description;
                sheet2.Cells[$"B{index}"].Value = tasks[i].Description;
                sheet2.Cells[$"C{index}"].Value = tasks[i].Id;
            }
        }
        private void FillHolidays(Tuple<DateTime, DateTime> period)
        {
            var dateFrom = period.Item1;
            var dateTo = period.Item2;

            Holidays = unitOfWork.HolidayRepository.Get(dateFrom.Year, dateFrom.Month);

            var holidaysNextMonth = unitOfWork.HolidayRepository.Get(dateTo.Year, dateTo.Month);

            foreach (var holiday in holidaysNextMonth)
            {
                Holidays.Add(holiday);
            }
        }

        private Task<MemoryStream> GetTemplateStream()
        {
            var fileRoot = new FileInfo($"{hostingEnvironment.ContentRootPath}/wwwroot/excelTemplates/worktime-template.xlsx");

            var template = new ExcelPackage(fileRoot, false);

            var memoryStream = new MemoryStream(template.GetAsByteArray());

            template.Dispose();

            return Task.FromResult(memoryStream);
        }
    }
}
