using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Sofco.Core.DAL;
using Sofco.Core.Data.Admin;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Domain.Models.WorkTimeManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sofco.Framework.FileManager.WorkTime
{
    public class WorkTimeExportFileManager : IWorkTimeExportFileManager
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<WorkTimeExportFileManager> logger;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IUserData userData;

        private IList<Holiday> Holidays { get; set; }

        public WorkTimeExportFileManager(IUnitOfWork unitOfWork, ILogMailer<WorkTimeExportFileManager> logger, IHostingEnvironment hostingEnvironment
                                        , IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.hostingEnvironment = hostingEnvironment;
            this.userData = userData;
        }

        public ExcelPackage CreateTemplateExcel(int analyticId, int periodId)
        {
            var memoryStream = this.GetTemplateStream().Result;

            var excel = new ExcelPackage(memoryStream);

            return Create(excel, analyticId, periodId);
        }

        private ExcelPackage Create(ExcelPackage excel, int analyticId, int periodId)
        {
            var sheet1 = excel.Workbook.Worksheets[1];
            var sheet2 = excel.Workbook.Worksheets[2];

            FillCategories(sheet2);
            FillResources(sheet1, analyticId, periodId);

            return excel;
        }

        private void FillResources(ExcelWorksheet sheet1, int analyticId, int periodId)
        {
            var closeDates = unitOfWork.CloseDateRepository.GetBeforeAndCurrent(periodId);

            var periodExcludeDays = new Tuple<DateTime, DateTime>(new DateTime(closeDates.Item2.Year, closeDates.Item2.Month, 1, 0, 0, 0),
                                                                  new DateTime(closeDates.Item1.Year, closeDates.Item1.Month, 1, 0, 0, 0));

            var period = new Tuple<DateTime, DateTime>(new DateTime(closeDates.Item2.Year, closeDates.Item2.Month, closeDates.Item2.Day+1, 0, 0, 0),
                                                       new DateTime(closeDates.Item1.Year, closeDates.Item1.Month, closeDates.Item1.Day, 0, 0, 0));

            try
            {
                FillHolidays(period);

                var employees = unitOfWork.AnalyticRepository.GetResources(analyticId, periodExcludeDays.Item1.Date, periodExcludeDays.Item2.Date);

                var manager = unitOfWork.AnalyticRepository.GetManager(analyticId);
                var director = unitOfWork.AnalyticRepository.GetDirector(analyticId);

                //Si no es director ni gerente es aprobador
                if (manager.Id != userData.GetCurrentUser().Id && director.Id != userData.GetCurrentUser().Id)
                {
                    //Busca los empleados que tiene para aprobar
                    var users = unitOfWork.UserApproverRepository.GetByAnalyticAndApproverUserId(userData.GetCurrentUser().Id, analyticId, Domain.Enums.UserApproverType.WorkTime);

                    employees = employees.Where(d => users.Select(u => u.EmployeeId).Contains(d.Id)).ToList();
                }

                var index = 2;

                var format = sheet1.Cells[$"C2"].Style.Numberformat.Format;

                foreach (var employee in employees)
                {
                    var startDate = period.Item1;
                    var endDate = period.Item2;

                    var licenses = unitOfWork.LicenseRepository.GetByEmployeeAndDates(employee.Id, startDate.Date, endDate.Date);

                    while (startDate.Date <= endDate.Date)
                    {
                        var firstDayOffMonth = new DateTime(startDate.Year, startDate.Month, 1);
                        bool hasLicence = licenses.Any(x => startDate.Date >= x.StartDate.Date && startDate.Date <= x.EndDate.Date);

                        if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday &&
                        Holidays.All(x => x.Date.Date != startDate.Date) && !hasLicence)
                        {
                            if (employee.Allocations.Any(x => x.StartDate.Date == firstDayOffMonth.Date && x.Percentage > 0 && x.AnalyticId == analyticId) &&
                                startDate.Date >= employee.StartDate.Date)
                            {
                                if (employee.EndDate.HasValue)
                                {
                                    if (startDate.Date <= employee.EndDate.Value.Date)
                                    {
                                        sheet1.Cells[$"A{index}"].Value = employee.EmployeeNumber;
                                        sheet1.Cells[$"B{index}"].Value = employee.Name;
                                        sheet1.Cells[$"C{index}"].Value = startDate.Date;
                                        sheet1.Cells[$"C{index}"].Style.Numberformat.Format = format;
                                        index++;
                                    }
                                }
                                else
                                {
                                    sheet1.Cells[$"A{index}"].Value = employee.EmployeeNumber;
                                    sheet1.Cells[$"B{index}"].Value = employee.Name;
                                    sheet1.Cells[$"C{index}"].Value = startDate.Date;
                                    sheet1.Cells[$"C{index}"].Style.Numberformat.Format = format;
                                    index++;
                                }
                            }
                        }

                        startDate = startDate.AddDays(1);
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e);
                throw e;
            }
        }

        private void FillCategories(ExcelWorksheet sheet2)
        {
            var tasks = unitOfWork.TaskRepository.GetAllActivesWithCategories().Where(x => !x.Category.Description.Equals("No Laborable")).ToList();

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
