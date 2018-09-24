using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.WorktimeManagement;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
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

            var daysoff = unitOfWork.HolidayRepository.Get(parameters.StartYear, parameters.StartMonth);
            daysoff.AddRange(unitOfWork.HolidayRepository.Get(parameters.EndYear, parameters.EndMonth));

            var allocations = unitOfWork.AllocationRepository.GetAllocationsForWorktimeReport(parameters);

            response.Data = new WorkTimeReportModel { Items = new List<WorkTimeReportModelItem>() };

            var model = new WorkTimeReportModelItem();
            var mustAddModel = false;

            foreach (var allocation in allocations)
            {
                if (allocation.Analytic == null || allocation.Employee == null || allocation.Analytic.Manager == null)
                    continue;

                //if (allocation.Percentage == 0) continue;

                if (model.EmployeeId == allocation.EmployeeId && model.AnalyticId == allocation.AnalyticId)
                {
                    model.HoursMustLoad += CalculateHoursToLoad(allocation, startDate, endDate, daysoff);
                    model.TotalPercentage += allocation.Percentage;
                    model.MonthPercentage += $" - {DatesHelper.GetDateDescription(allocation.StartDate)} {Math.Round(allocation.Percentage, 0)}%";
                }
                else
                {
                    var modelAlreadyExist = response.Data.Items.SingleOrDefault(x => x.EmployeeId == allocation.EmployeeId && x.AnalyticId == allocation.AnalyticId);

                    if (modelAlreadyExist != null)
                    {
                        modelAlreadyExist.HoursMustLoad += CalculateHoursToLoad(allocation, startDate, endDate, daysoff);
                        modelAlreadyExist.TotalPercentage += allocation.Percentage;
                        modelAlreadyExist.Result = modelAlreadyExist.HoursLoaded >= modelAlreadyExist.HoursMustLoad;
                        modelAlreadyExist.MonthPercentage += $" - {DatesHelper.GetDateDescription(allocation.StartDate)} {Math.Round(allocation.Percentage, 0)}% ";
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
                            Client = allocation.Analytic.ClientExternalName,
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
                            TotalPercentage = allocation.Percentage,
                            MonthPercentage = $"{DatesHelper.GetDateDescription(allocation.StartDate)} {Math.Round(allocation.Percentage, 0)}% ",
                            HoursLoaded = unitOfWork.WorkTimeRepository.GetTotalHoursBetweenDays(allocation.EmployeeId, startDate, endDate, allocation.AnalyticId)
                        };

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

            var tigerReport = new List<TigerReportItem>();

            SaveTigerTxt(response, tigerReport);

            if (!response.Data.Items.Any())
            {
                response.AddWarning(Resources.WorkTimeManagement.WorkTime.SearchNotFound);
            }
            else
            {
                response.Data.IsCompleted = response.Data.Items.All(x => x.HoursLoadedSuccesfully && !x.MissAnyPercentageAllocation);

                worktimeData.ClearKeys();
                worktimeData.SaveTigerReport(tigerReport);
            }

            return response;
        }

        private static void CalculateRealPercentage(Response<WorkTimeReportModel> response)
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
                    item.RealPercentage = Math.Round((percentageWithoutRound * (item.HoursLoaded * 100 / item.HoursMustLoad) / 100), MidpointRounding.AwayFromZero);
                }
            }
        }

        private static void SaveTigerTxt(Response<WorkTimeReportModel> response, List<TigerReportItem> tigerReport)
        {
            var i = 1;
            foreach (var item in response.Data.Items)
            {
                var sumPercentage = response.Data.Items.Where(x => x.EmployeeId == item.EmployeeId).Select(x => x.RealPercentage).Sum();

                if (item.Facturability > 0)
                {
                    if (sumPercentage == 100) item.HoursLoadedSuccesfully = true;

                    var sumTotalPercentage = response.Data.Items.Where(x => x.EmployeeId == item.EmployeeId).Select(x => x.TotalPercentage).Sum();

                    if (sumTotalPercentage != 200)
                    {
                        item.MissAnyPercentageAllocation = true;
                    }
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

            if (allocation.Employee.BillingPercentage == 0)
            {
                return Math.Round((businessDays * allocation.Employee.BusinessHours * allocation.Percentage) / 100);
            }
            else
            {
                return Math.Round((businessDays * allocation.Employee.BusinessHours * allocation.Percentage * (allocation.Employee.BillingPercentage / 100)) / 100);
            }
        }
    }
}
