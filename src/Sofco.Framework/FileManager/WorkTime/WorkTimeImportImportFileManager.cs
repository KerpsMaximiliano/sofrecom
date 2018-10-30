using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Core.Models.Common;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;

namespace Sofco.Framework.FileManager.WorkTime
{
    public class WorkTimeImportImportFileManager : IWorkTimeImportFileManager
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<WorkTimeImportImportFileManager> logger;
        private readonly IUserData userData;

        private IList<int> TaskIds { get; set; }

        private IList<Employee> Employees { get; set; }

        private IList<Holiday> Holidays { get; set; }

        private IList<Domain.Models.WorkTimeManagement.WorkTime> WorkTimesToAdd { get; set; }

        private IDictionary<string, int> UserMails { get; set; }

        private DateTime DateFrom { get; set; }

        private DateTime DateTo { get; set; }

        private int RowsAdded { get; set; }

        public WorkTimeImportImportFileManager(IUnitOfWork unitOfWork, ILogMailer<WorkTimeImportImportFileManager> logger, IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.userData = userData;

            WorkTimesToAdd = new List<Domain.Models.WorkTimeManagement.WorkTime>();
            UserMails = new Dictionary<string, int>();
        }

        public void Import(int analyticId, MemoryStream memoryStream, Response<IList<WorkTimeImportResult>> response)
        {
            var settingHour = unitOfWork.SettingRepository.GetByKey(SettingConstant.WorkingHoursPerDaysMaxKey);
            var closeDates = unitOfWork.CloseDateRepository.GetBeforeCurrentAndNext();
             
            TaskIds = unitOfWork.TaskRepository.GetAllIds();
            Employees = unitOfWork.EmployeeRepository.GetByAnalyticWithWorkTimes(analyticId);
            FillHolidays(closeDates);
            RowsAdded = 0;
         
            response.Data = new List<WorkTimeImportResult>();

            var excel = new ExcelPackage(memoryStream);

            var sheet = excel.Workbook.Worksheets.First();

            var end = false;
            var i = 2;

            while (!end)
            {
                var employeeNumber = sheet.GetValue(i, 1)?.ToString();

                if (string.IsNullOrWhiteSpace(employeeNumber))
                {
                    end = true;
                    continue;
                }

                try
                {
                    var employeeDesc = sheet.GetValue(i, 2)?.ToString();
                    var date = sheet.GetValue(i, 3)?.ToString();
                    var taskId = sheet.GetValue(i, 4)?.ToString();
                    var hour = sheet.GetValue(i, 5)?.ToString();
                    var comments = sheet.GetValue(i, 6)?.ToString();

                    if (IsValidDate(date, out var datetime)) date = datetime.ToString("d");
               
                    var employee = Employees.SingleOrDefault(x => x.EmployeeNumber.Equals(employeeNumber));

                    if (!ValidateEmployee(response, employee, employeeNumber, i, employeeDesc, date)) continue;

                    if (!ValidateDate(response, employee, date, i, employeeNumber, employeeDesc)) continue;

                    if (!ValidateHours(response, employee, date, settingHour, i, employeeNumber, employeeDesc, hour)) continue;

                    if (!ValidateTask(response, taskId, i, employeeNumber, employeeDesc, date)) continue;

                    AddWorkTime(analyticId, response, i, employeeNumber, employeeDesc, date, taskId, hour, comments, employee);
                }
                catch (Exception e)
                {
                    var item = FillItemResult(i, employeeNumber, string.Empty, string.Empty);
                    item.Error = Resources.Common.GeneralError;
                    response.Data.Add(item);
                    logger.LogError(e);
                }
                finally
                {
                    i++;
                }
            }

            if (!response.Data.Any())
            {
                if (RowsAdded == 0)
                {
                    response.AddError(Resources.WorkTimeManagement.WorkTime.FileEmpty);
                    return;
                }

                try
                {
                    unitOfWork.BeginTransaction();
                    unitOfWork.WorkTimeRepository.InsertBulk(WorkTimesToAdd);
                    unitOfWork.Commit();
                }
                catch (Exception e)
                {
                    response.AddError(Resources.Common.ErrorSave);
                    logger.LogError(e);
                }
            }
        }

        private void AddWorkTime(int analyticId, Response<IList<WorkTimeImportResult>> response, int i, string employeeNumber, string employeeDesc, string date, string taskId, string hour, string comments, Employee employee)
        {
            var worktime = new Domain.Models.WorkTimeManagement.WorkTime();

            worktime.AnalyticId = analyticId;
            worktime.EmployeeId = employee.Id;
            worktime.Date = Convert.ToDateTime(date);
            worktime.TaskId = Convert.ToInt32(taskId);
            worktime.Hours = Convert.ToDecimal(hour);
            worktime.CreationDate = DateTime.UtcNow;
            worktime.Source = WorkTimeSource.MassiveImport.ToString();
            worktime.UserComment = comments;
            worktime.Status = WorkTimeStatus.Approved;
            worktime.ApprovalUserId = userData.GetCurrentUser().Id;

            if (UserMails.ContainsKey(employee.Email))
            {
                var userId = UserMails[employee.Email];
                worktime.UserId = userId;
                WorkTimesToAdd.Add(worktime);
            }
            else
            {
                var user = unitOfWork.UserRepository.GetByEmail(employee.Email);
                if (user != null)
                {
                    UserMails.Add(employee.Email, user.Id);
                    worktime.UserId = user.Id;
                    WorkTimesToAdd.Add(worktime);
                    RowsAdded++;
                }
                else
                {
                    var item = FillItemResult(i, employeeNumber, employeeDesc, date);
                    item.Error = Resources.WorkTimeManagement.WorkTime.ImportUserWithEmailNull;
                    response.Data.Add(item);
                }
            }
        }

        private bool ValidateTask(Response<IList<WorkTimeImportResult>> response, string taskId, int i, string employeeNumber, string employeeDesc,
            string date)
        {
            if (!TaskIds.Any(x => x.ToString().Equals(taskId)))
            {
                var item = FillItemResult(i, employeeNumber, employeeDesc, date);
                item.Error = Resources.WorkTimeManagement.WorkTime.ImportTaskNoExist;
                response.Data.Add(item);

                return false;
            }

            return true;
        }

        private bool ValidateHours(Response<IList<WorkTimeImportResult>> response, Employee employee, string date, Setting settingHour, int i,
            string employeeNumber, string employeeDesc, string hour)
        {
            if (employee != null)
            {
                if (string.IsNullOrWhiteSpace(hour))
                {
                    var item = FillItemResult(i, employeeNumber, employeeDesc, date);
                    item.Error = Resources.WorkTimeManagement.WorkTime.ImportHoursNull;
                    response.Data.Add(item);

                    return false;
                }

                var datetime = Convert.ToDateTime(date);

                var hoursAlreadyAdded = WorkTimesToAdd.Where(x => x.EmployeeId == employee.Id && x.Date.Date == datetime.Date).Sum(x => x.Hours);

                decimal hours = 0;

                if (employee.WorkTimes != null)
                {
                    hours = employee.WorkTimes.Where(x => x.Date.Date == datetime.Date).Sum(x => x.Hours);
                }

                if (hours + Convert.ToDecimal(hour) + hoursAlreadyAdded > Convert.ToDecimal(settingHour.Value))
                {
                    var item = FillItemResult(i, employeeNumber, employeeDesc, date);
                    item.Error = Resources.WorkTimeManagement.WorkTime.ImportHoursExceed;
                    response.Data.Add(item);

                    return false;
                }
            }

            return true;
        }

        private bool ValidateEmployee(Response<IList<WorkTimeImportResult>> response, Employee employee, string employeeNumber, int i,
            string employeeDesc, string date)
        {
            if (employee == null)
            {
                if (!response.Data.Any(x => x.EmployeeNumber.Equals(employeeNumber)))
                {
                    var item = FillItemResult(i, employeeNumber, employeeDesc, date);
                    item.Error = Resources.WorkTimeManagement.WorkTime.ImportEmployeeIsNotInAnalytic;
                    response.Data.Add(item);
                }

                return false;
            }

            return true;
        }

        private bool IsValidDate(string date, out DateTime datetime)
        {
            datetime = DateTime.MinValue;

            if (string.IsNullOrWhiteSpace(date)) return false;
            if (date.Length != 10) return false;

            var split = date.Split('/');
            if (split.Length != 3) return false;

            try
            {
                datetime = new DateTime(Convert.ToInt32(split[2]), Convert.ToInt32(split[1]), Convert.ToInt32(split[0]));
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private bool ValidateDate(Response<IList<WorkTimeImportResult>> response, Employee employee, string date, int i, string employeeNumber, string employeeDesc)
        {
            if (!IsValidDate(date, out var datetime))
            {
                var item = FillItemResult(i, employeeNumber, employeeDesc, date);
                item.Error = Resources.WorkTimeManagement.WorkTime.ImportDateNull;
                response.Data.Add(item);

                return false;
            }
            else
            {
                if (datetime.Date >= DateFrom.Date && datetime.Date <= DateTo.Date)
                {
                    if (datetime.DayOfWeek == DayOfWeek.Saturday || datetime.DayOfWeek == DayOfWeek.Sunday ||
                        Holidays.Any(x => x.Date.Date == datetime.Date))
                    {
                        var item = FillItemResult(i, employeeNumber, employeeDesc, datetime.ToString("d"));
                        item.Error = Resources.WorkTimeManagement.WorkTime.ImportDateWrong;
                        response.Data.Add(item);

                        return false;
                    }

                    if (employee.Licenses.Any(x =>
                        datetime.Date >= x.StartDate.Date && datetime.Date <= x.EndDate.Date))
                    {
                        var item = FillItemResult(i, employeeNumber, employeeDesc, datetime.ToString("d"));
                        item.Error = Resources.WorkTimeManagement.WorkTime.ImportEmployeeWithLicense;
                        response.Data.Add(item);

                        return false;
                    }
                }
                else
                {
                    var item = FillItemResult(i, employeeNumber, employeeDesc, datetime.ToString("d"));
                    item.Error = Resources.WorkTimeManagement.WorkTime.ImportDatesOutOfRange;
                    response.Data.Add(item);

                    return false;
                }
            }

            return true;
        }

        private static WorkTimeImportResult FillItemResult(int i, string employeeNumber, string employeeDesc, string date)
        {
            return new WorkTimeImportResult
            {
                EmployeeNumber = employeeNumber,
                Employee = employeeDesc,
                Date = date,
                Row = $"Fila {i}"
            };
        }

        private void FillHolidays(CloseDatesSettings closeDatesSettings)
        {
            var period = closeDatesSettings.GetPeriodIncludeDays();

            DateFrom = period.Item1;
            DateTo = period.Item2;

            Holidays = unitOfWork.HolidayRepository.Get(DateFrom.Year, DateFrom.Month);

            var holidaysNextMonth = unitOfWork.HolidayRepository.Get(DateTo.Year, DateTo.Month);

            foreach (var holiday in holidaysNextMonth)
            {
                Holidays.Add(holiday);
            }
        }
    }
}
