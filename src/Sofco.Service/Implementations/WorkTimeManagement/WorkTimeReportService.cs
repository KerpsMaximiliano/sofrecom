using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.WorktimeManagement;
using Sofco.Core.DAL;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;

namespace Sofco.Service.Implementations.WorkTimeManagement
{
    public class WorkTimeReportService : IWorkTimeReportService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IWorktimeData worktimeData;

        private int LastMonthAllocation { get; set; }

        private int CurrentMonthAllocation { get; set; }

        public WorkTimeReportService(IUnitOfWork unitOfWork,
            IWorktimeData worktimeData)
        {
            this.unitOfWork = unitOfWork;
            this.worktimeData = worktimeData;
        }

        public Response<WorkTimeReportModel> CreateReport(ReportParams parameters)
        {
            var response = new Response<WorkTimeReportModel>();

            if (parameters.CloseMonthId <= 0)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.YearAndMonthRequired);
                return response;
            }

            var closeDates = unitOfWork.CloseDateRepository.GetBeforeAndCurrent(parameters.CloseMonthId);

            var startDate = new DateTime(closeDates.Item2.Year, closeDates.Item2.Month, closeDates.Item2.Day + 1);
            var endDate = new DateTime(closeDates.Item1.Year, closeDates.Item1.Month, closeDates.Item1.Day);

            parameters.StartYear = startDate.Year;
            parameters.StartMonth = startDate.Month;
            parameters.EndYear = endDate.Year;
            parameters.EndMonth = endDate.Month;

            LastMonthAllocation = startDate.Month;
            CurrentMonthAllocation = endDate.Month;

            var daysoff = unitOfWork.HolidayRepository.Get(parameters.StartYear, parameters.StartMonth);
            daysoff.AddRange(unitOfWork.HolidayRepository.Get(parameters.EndYear, parameters.EndMonth));

            var allocations = unitOfWork.AllocationRepository.GetAllocationsForWorkTimeReport(parameters);

            response.Data = new WorkTimeReportModel { Items = new List<WorkTimeReportModelItem>() };

            response.Data.EmployeesAllocationResume = new List<EmployeeAllocationResume>();
            var model = new WorkTimeReportModelItem();
            var mustAddModel = false;

            foreach (var allocation in allocations)
            {
                if (allocation.Analytic == null || allocation.Employee == null || allocation.Analytic.Manager == null)
                    continue;

                if (allocation.Percentage == 0)
                {
                    CalculateEmployeesAllocationResume(response, allocation);
                    continue;
                }

                if (model.EmployeeId == allocation.EmployeeId && model.AnalyticId == allocation.AnalyticId)
                {
                    model.HoursMustLoad += CalculateHoursToLoad(allocation, startDate, endDate, daysoff);

                    CalculateEmployeesAllocationResume(response, allocation);
                }
                else
                {
                    var modelAlreadyExist = response.Data.Items.SingleOrDefault(x => x.EmployeeId == allocation.EmployeeId && x.AnalyticId == allocation.AnalyticId);

                    if (modelAlreadyExist != null)
                    {
                        modelAlreadyExist.HoursMustLoad += CalculateHoursToLoad(allocation, startDate, endDate, daysoff);
                        modelAlreadyExist.Result = modelAlreadyExist.HoursLoaded >= modelAlreadyExist.HoursMustLoad;

                        CalculateEmployeesAllocationResume(response, allocation);
                    }
                    else
                    {
                        if (mustAddModel)
                        {
                            model.Result = model.HoursLoaded >= model.HoursMustLoad;
                            response.Data.Items.Add(model);
                        }

                        model = new WorkTimeReportModelItem
                        {
                            Client = allocation.Analytic.AccountName,
                            AnalyticId = allocation.AnalyticId,
                            Analytic = $"{allocation.Analytic.Title} - {allocation.Analytic.Name}",
                            Title = $"{allocation.Analytic.Title}",
                            Manager = allocation.Analytic.Manager.Name,
                            EmployeeId = allocation.Employee.Id,
                            EmployeeNumber = allocation.Employee.EmployeeNumber,
                            Employee = allocation.Employee.Name,
                            CostCenter = allocation.Analytic.CostCenter?.Code,
                            Activity = allocation.Analytic.Activity?.Text,
                            Facturability = allocation.Employee.BillingPercentage,
                            HoursLoaded = unitOfWork.WorkTimeRepository.GetTotalHoursBetweenDays(allocation.EmployeeId, startDate, endDate, allocation.AnalyticId),
                        };

                        CalculateEmployeesAllocationResume(response, allocation);

                        model.HoursMustLoad += CalculateHoursToLoad(allocation, startDate, endDate, daysoff);

                        mustAddModel = true;
                    }
                }
            }

            if (mustAddModel)
            {
                model.Result = model.HoursLoaded >= model.HoursMustLoad;
                response.Data.Items.Add(model);
            }

            CalculateRealPercentage(response);

            if (parameters.ExportTigerVisible)
            {
                FillUnassigned(response, startDate, endDate);
            }

            if (!response.Data.Items.Any())
            {
                response.AddWarning(Resources.WorkTimeManagement.WorkTime.SearchNotFound);
            }
            else
            {
                var tigerReport = new List<TigerReportItem>();

                SaveTigerTxt(response, tigerReport);

                response.Data.IsCompleted = response.Data.Items.All(x => x.HoursLoadedSuccesfully) && response.Data.EmployeesAllocationResume.All(x => !x.MissAnyPercentageAllocation);

                worktimeData.ClearTigerReportKey();
                worktimeData.SaveTigerReport(tigerReport);
            }

            return response;
        }

        private void FillUnassigned(Response<WorkTimeReportModel> response, DateTime startDate, DateTime endDate)
        {
            var employeesUnassigned = unitOfWork.EmployeeRepository.GetUnassignedBetweenDays(startDate, endDate);

            foreach (var employee in employeesUnassigned)
            {
                var employeeMissAllocation = response.Data.EmployeesAllocationResume.SingleOrDefault(x => x.EmployeeId == employee.Id);

                if (employeeMissAllocation == null)
                {
                    var itemToAdd = new EmployeeAllocationResume
                    {
                        EmployeeId = employee.Id,
                        Employee = employee.Name,
                        LastPercentage = 0,
                        CurrentPercentage = 0,
                        LastMonth = startDate.Month,
                        CurrentMonth = endDate.Month,
                        LastMonthDescription = DatesHelper.GetDateDescription(startDate),
                        CurrentMonthDescription = DatesHelper.GetDateDescription(endDate)
                    };

                    response.Data.EmployeesAllocationResume.Add(itemToAdd);
                }
                else
                {
                    if (employeeMissAllocation.LastMonth == 0)
                    {
                        employeeMissAllocation.LastPercentage = 0;
                        employeeMissAllocation.LastMonth = startDate.Month;
                        employeeMissAllocation.LastMonthDescription = DatesHelper.GetDateDescription(startDate);
                    }

                    if (employeeMissAllocation.CurrentMonth == 0)
                    {
                        employeeMissAllocation.CurrentPercentage = 0;
                        employeeMissAllocation.CurrentMonth = endDate.Month;
                        employeeMissAllocation.CurrentMonthDescription = DatesHelper.GetDateDescription(endDate);
                    }
                }
            }
        }

        private void CalculateEmployeesAllocationResume(Response<WorkTimeReportModel> response, Allocation allocation)
        {
            var employeeMissAllocation = response.Data.EmployeesAllocationResume.SingleOrDefault(x => x.EmployeeId == allocation.EmployeeId);

            if (employeeMissAllocation == null)
            {
                var itemToAdd = new EmployeeAllocationResume
                {
                    EmployeeId = allocation.EmployeeId,
                    Employee = allocation.Employee.Name
                };

                if (allocation.StartDate.Month == LastMonthAllocation)
                {
                    itemToAdd.LastMonth = allocation.StartDate.Month;
                    itemToAdd.LastMonthDescription = DatesHelper.GetDateDescription(allocation.StartDate);
                    itemToAdd.LastPercentage += allocation.Percentage;
                }
                else
                {
                    itemToAdd.CurrentMonth = allocation.StartDate.Month;
                    itemToAdd.CurrentMonthDescription = DatesHelper.GetDateDescription(allocation.StartDate);
                    itemToAdd.CurrentPercentage += allocation.Percentage;
                }

                response.Data.EmployeesAllocationResume.Add(itemToAdd);
            }
            else
            {
                employeeMissAllocation.EmployeeId = allocation.EmployeeId;
                employeeMissAllocation.Employee = allocation.Employee.Name;

                if (allocation.StartDate.Month == LastMonthAllocation)
                {
                    employeeMissAllocation.LastMonth = allocation.StartDate.Month;
                    employeeMissAllocation.LastMonthDescription = DatesHelper.GetDateDescription(allocation.StartDate);
                    employeeMissAllocation.LastPercentage += allocation.Percentage;
                }
                else
                {
                    employeeMissAllocation.CurrentMonth = allocation.StartDate.Month;
                    employeeMissAllocation.CurrentMonthDescription = DatesHelper.GetDateDescription(allocation.StartDate);
                    employeeMissAllocation.CurrentPercentage += allocation.Percentage;
                }
            }
        }

        private void CalculateRealPercentage(Response<WorkTimeReportModel> response)
        {
            foreach (var item in response.Data.Items)
            {
                var allHoursMustLoaded = response.Data.Items.Where(x => x.EmployeeId == item.EmployeeId).Select(x => x.HoursMustLoad).Sum();

                item.AllocationPercentage = Math.Round(item.HoursMustLoad * 100 / allHoursMustLoaded, MidpointRounding.AwayFromZero);
                var percentageWithoutRound = item.HoursMustLoad * 100 / allHoursMustLoaded;

                if (item.Facturability == 0)
                {
                    item.HoursMustLoad = 0;
                    item.Result = true;
                    item.RealPercentage = item.AllocationPercentage;
                }
                else
                {
                    if (item.HoursMustLoad != 0)
                    {
                        item.RealPercentage = Math.Round((percentageWithoutRound * (item.HoursLoaded * 100 / item.HoursMustLoad) / 100), MidpointRounding.AwayFromZero);
                    }
                }
            }
        }

        private void SaveTigerTxt(Response<WorkTimeReportModel> response, List<TigerReportItem> tigerReport)
        {
            var i = 1;
            foreach (var item in response.Data.Items)
            {
                var sumPercentage = response.Data.Items.Where(x => x.EmployeeId == item.EmployeeId).Select(x => x.RealPercentage).Sum();

                if (item.Facturability > 0)
                {
                    if (sumPercentage >= 100) item.HoursLoadedSuccesfully = true;
                }
                else
                {
                    item.HoursLoadedSuccesfully = true;
                }

                var tigerItem = new TigerReportItem(item.EmployeeNumber, item.RealPercentage, item.CostCenter, item.Activity, item.Title) { Id = i };
                i++;

                tigerReport.Add(tigerItem);
            }
        }

        private decimal CalculateHoursToLoad(Allocation allocation, DateTime startDate, DateTime endDate, IList<Holiday> holidays)
        {
            var businessDays = 0;

            while (startDate.Date <= endDate.Date)
            {
                if (allocation.StartDate.Month == startDate.Month)
                {
                    if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday && holidays.All(x => x.Date.Date != startDate.Date))
                        businessDays++;
                }

                startDate = startDate.AddDays(1);
            }

            return Math.Round((businessDays * allocation.Employee.BusinessHours * allocation.Percentage) / 100);
        }
    }
}
