﻿using Sofco.Core.DAL;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.WorktimeManagement;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.Domain;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Domain.Enums;

namespace Sofco.Service.Implementations.WorkTimeManagement
{
    public class WorkTimeReportService : IWorkTimeReportService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IWorktimeData worktimeData;

        private int LastMonthAllocation { get; set; }

        private int CurrentMonthAllocation { get; set; }

        private readonly bool workTimeReportByHours;

        public WorkTimeReportService(IUnitOfWork unitOfWork,
            IWorktimeData worktimeData,
            ISettingData settingData)
        {
            this.unitOfWork = unitOfWork;
            this.worktimeData = worktimeData;

            var validatePeriodSetting = settingData.GetByKey(SettingConstant.WorkTimeReportByHours);
            if (validatePeriodSetting != null)
            {
                workTimeReportByHours = bool.Parse(validatePeriodSetting.Value);
            }
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

            var allocations = unitOfWork.AllocationRepository.GetAllocationsForWorkTimeReport(parameters, startDate, endDate);

            response.Data = new WorkTimeReportModel {Items = new List<WorkTimeReportModelItem>()};

            //var employeesToRecalculate = new List<EmployeeToRecalculate>();
            var employeesMissingHours = new List<EmployeeMissingHours>();

            response.Data.EmployeesAllocationResume = new List<EmployeeAllocationResume>();
            var model = new WorkTimeReportModelItem();
            var mustAddModel = false;

            foreach (var allocation in allocations)
            {
                if (allocation.Analytic == null || allocation.Employee == null || allocation.Analytic.Manager == null)
                    continue;

                if (allocation.Percentage == 0)
                {
                    CalculateEmployeesAllocationResume(response, allocation, parameters, model);
                    continue;
                }

                if (model.EmployeeId == allocation.EmployeeId && model.AnalyticId == allocation.AnalyticId)
                {
                    var tuple = CalculateHoursToLoad(allocation, startDate, endDate, daysoff);
                    model.HoursMustLoad += tuple.Item1;
                    model.AllHoursMustLoad = tuple.Item2;

                    CalculateEmployeesAllocationResume(response, allocation, parameters, model);

                    // Guardo analiticas de preventa
                    //if (allocation.AnalyticId == 146 || allocation.AnalyticId == 166 || allocation.AnalyticId == 167)
                    //{
                    //    var emp = employeesToRecalculate.FirstOrDefault(x => x.EmployeeId == allocation.EmployeeId);

                    //    if (emp != null)
                    //    {
                    //        emp.Percentage += allocation.Percentage;
                    //    }
                    //    else
                    //    {
                    //        employeesToRecalculate.Add(new EmployeeToRecalculate
                    //            {EmployeeId = allocation.EmployeeId, Percentage = allocation.Percentage, Count = 1});
                    //    }
                    //}
                }
                else
                {
                    var modelAlreadyExist = response.Data.Items.SingleOrDefault(x =>
                        x.EmployeeId == allocation.EmployeeId && x.AnalyticId == allocation.AnalyticId);

                    if (modelAlreadyExist != null)
                    {
                        var tuple = CalculateHoursToLoad(allocation, startDate, endDate, daysoff);
                        modelAlreadyExist.HoursMustLoad += tuple.Item1;
                        modelAlreadyExist.AllHoursMustLoad = tuple.Item2;

                        modelAlreadyExist.Result = modelAlreadyExist.HoursApproved >= modelAlreadyExist.HoursMustLoad;

                        CalculateEmployeesAllocationResume(response, allocation, parameters, modelAlreadyExist);

                        //if (allocation.AnalyticId == 146 || allocation.AnalyticId == 166 ||
                        //    allocation.AnalyticId == 167)
                        //{
                        //    var emp = employeesToRecalculate.FirstOrDefault(x => x.EmployeeId == allocation.EmployeeId);

                        //    if (emp != null)
                        //    {
                        //        emp.Percentage += allocation.Percentage;
                        //    }
                        //    else
                        //    {
                        //        employeesToRecalculate.Add(new EmployeeToRecalculate
                        //        {
                        //            EmployeeId = allocation.EmployeeId, Percentage = allocation.Percentage, Count = 1
                        //        });
                        //    }
                        //}
                    }
                    else
                    {
                        if (mustAddModel)
                        {
                            model.Result = model.HoursApproved >= model.HoursMustLoad;
                            response.Data.Items.Add(model);
                        }

                        var worktimes = unitOfWork.WorkTimeRepository.GetTotalHoursBetweenDays(allocation.EmployeeId,
                            startDate, endDate, allocation.AnalyticId);

                        model = new WorkTimeReportModelItem
                        {
                            Client = allocation.Analytic.AccountName,
                            AnalyticId = allocation.AnalyticId,
                            Analytic = $"{allocation.Analytic.Title} - {allocation.Analytic.Name}",
                            Title = $"{allocation.Analytic.Title}",
                            Manager = allocation.Analytic.Manager.Name,
                            ManagerId = allocation.Analytic.ManagerId.GetValueOrDefault(),
                            EmployeeId = allocation.Employee.Id,
                            EmployeeNumber = allocation.Employee.EmployeeNumber,
                            Employee = allocation.Employee.Name,
                            CostCenter = allocation.Analytic.CostCenter?.Code,
                            Activity = allocation.Analytic.Activity?.Text,
                            Facturability = allocation.Employee.BillingPercentage,
                            HoursLoaded = worktimes.Sum(s => s.Hours),
                            DraftHours = worktimes.Where(s => s.Status == WorkTimeStatus.Draft).Sum(s => s.Hours),
                            SentHours = worktimes.Where(s => s.Status == WorkTimeStatus.Sent).Sum(s => s.Hours),
                            HoursApproved = worktimes.Where(s => s.Status == WorkTimeStatus.Approved || s.Status == WorkTimeStatus.License).Sum(s => s.Hours)
                        };

                        if (parameters.EmployeeId.HasValue && parameters.EmployeeId.Value > 0 &&
                            allocation.Employee.EndDate.HasValue)
                        {
                            model.HasEndDate = true;
                        }

                        if (allocation.Employee.StartDate.Date > endDate.Date)
                        {
                            model.Facturability = 0;
                            model.RealPercentage = allocation.Percentage;
                            model.AllocationPercentage = allocation.Percentage == -1 ? 0 : allocation.Percentage;
                        }

                        CalculateEmployeesAllocationResume(response, allocation, parameters, model);

                        //if (allocation.AnalyticId == 146 || allocation.AnalyticId == 166 ||
                        //    allocation.AnalyticId == 167)
                        //{
                        //    var emp = employeesToRecalculate.FirstOrDefault(x => x.EmployeeId == allocation.EmployeeId);

                        //    if (emp != null)
                        //    {
                        //        emp.Percentage += allocation.Percentage;
                        //        emp.Count++;
                        //    }
                        //    else
                        //    {
                        //        employeesToRecalculate.Add(new EmployeeToRecalculate
                        //        {
                        //            EmployeeId = allocation.EmployeeId, Percentage = allocation.Percentage, Count = 1
                        //        });
                        //    }
                        //}

                        var tuple = CalculateHoursToLoad(allocation, startDate, endDate, daysoff);

                        model.HoursMustLoad += tuple.Item1;
                        model.AllHoursMustLoad = tuple.Item2;

                        if (employeesMissingHours.Any(x => x.EmployeeNumber == allocation.Employee.EmployeeNumber))
                        {
                            var employeeMissingHour = employeesMissingHours.FirstOrDefault(x => x.EmployeeNumber == allocation.Employee.EmployeeNumber);
                            if (employeeMissingHour != null)
                            {
                                employeeMissingHour.HoursApproved += model.HoursApproved;
                                employeeMissingHour.DraftHours += model.DraftHours;
                                employeeMissingHour.SentHours += model.SentHours;
                            }
                        }
                        else
                        {
                            var employeeMissing = new EmployeeMissingHours
                            {
                                Name = allocation.Employee.Name,
                                Manager = allocation.Employee?.Manager?.Name,
                                EmployeeNumber = allocation.Employee.EmployeeNumber,
                                HoursApproved = model.HoursApproved,
                                DraftHours = model.DraftHours,
                                SentHours = model.SentHours,
                                HoursMustLoad = tuple.Item2,
                                Facturability = allocation.Employee.BillingPercentage,
                                WorkTimeReportByHours = workTimeReportByHours
                            };

                            employeesMissingHours.Add(employeeMissing);
                        }

                        mustAddModel = true;
                    }
                }
            }

            if (mustAddModel)
            {
                model.Result = model.HoursApproved >= model.HoursMustLoad;
                response.Data.Items.Add(model);
            }

            //RemovePreventaAnalyticsHoursLoaded(response);

            CalculateRealPercentage(response);
            //RecalculatePreventa(response, employeesToRecalculate);

            if (parameters.ExportTigerVisible && !parameters.AnalyticId.Any() && !parameters.ManagerId.Any())
            {
                FillUnassigned(response, startDate, endDate, parameters.EmployeeId.GetValueOrDefault());
            }

            response.Data.EmployeesAllocationResume = response.Data.EmployeesAllocationResume.OrderBy(x => x.Employee).ToList();

            if (parameters.AnalyticId.Any() || parameters.ManagerId.Any())
            {
                if (parameters.AnalyticId.Any())
                {
                    response.Data.Items = response.Data.Items.Where(x => parameters.AnalyticId.Contains(x.AnalyticId)).ToList();
                }

                if (parameters.ManagerId.Any())
                {
                    response.Data.Items = response.Data.Items.Where(x => parameters.ManagerId.Contains(x.ManagerId)).ToList();
                }
            }
            else
            {
                response.Data.Items = response.Data.Items.OrderBy(x => x.Employee).ToList();
            }
            
            response.Data.WorkTimeReportByHours = workTimeReportByHours;
            response.Data.EmployeesMissingHours = employeesMissingHours.Where(x => x.MissingHours > 0).OrderBy(x => x.EmployeeNumber).ToList();

            if (parameters.EmployeeId.HasValue && parameters.EmployeeId.Value > 0 && response.Data.Items.All(x => x.HasEndDate))
            {
                response.Data.CanExportSingleTigerFile = true;
            }

            if (!response.Data.Items.Any())
            {
                response.AddWarning(Resources.WorkTimeManagement.WorkTime.SearchNotFound);
            }
            else
            {
                var tigerReport = new List<TigerReportItem>();

                SaveTigerTxt(response, tigerReport, new DateTime(parameters.EndYear, parameters.EndMonth, 1));

                response.Data.IsCompleted = (response.Data.EmployeesMissingHours == null || !response.Data.EmployeesMissingHours.Any()) &&
                                            response.Data.EmployeesAllocationResume.All(x =>
                                                !x.MissAnyPercentageAllocation);

                worktimeData.ClearTigerReportKey();
                worktimeData.SaveTigerReport(tigerReport.OrderBy(x => x.EmployeeNumber).ToList());
            }

            return response;
        }

        //private void RecalculatePreventa(Response<WorkTimeReportModel> response,
        //    List<EmployeeToRecalculate> employeesToRecalculate)
        //{
        //    foreach (var emp in employeesToRecalculate)
        //    {
        //        if (emp.Count == 0) continue;

        //        var percentageToRecalculate = emp.Percentage / emp.Count;

        //        var rows = response.Data.Items.Where(x => x.EmployeeId == emp.EmployeeId).ToList();

        //        if (percentageToRecalculate == 0) continue;

        //        if (!rows.Any()) continue;

        //        var index = 0;
        //        var end = false;

        //        while (!end)
        //        {
        //            var row = rows[index];

        //            if (row.AnalyticId != 146 && row.AnalyticId != 166 && row.AnalyticId != 167)
        //            {
        //                if (row.AllocationPercentage < 100)
        //                {
        //                    row.AllocationPercentage++;
        //                    percentageToRecalculate--;
        //                }
        //            }

        //            index++;

        //            if (index == rows.Count) index = 0;

        //            if (percentageToRecalculate == 0 || rows.Sum(x => x.AllocationPercentage) == 100)
        //                end = true;
        //        }
        //    }
        //}

        //private void RemovePreventaAnalyticsHoursLoaded(Response<WorkTimeReportModel> response)
        //{
        //    var preventaGrouped = response.Data.Items
        //        .Where(x => x.AnalyticId == 146 || x.AnalyticId == 166 || x.AnalyticId == 167)
        //        .GroupBy(x => x.EmployeeId, x => x.HoursLoaded).ToList();

        //    var preventaApprovedGrouped = response.Data.Items
        //        .Where(x => x.AnalyticId == 146 || x.AnalyticId == 166 || x.AnalyticId == 167)
        //        .GroupBy(x => x.EmployeeId, x => x.HoursApproved).ToList();

        //    foreach (var preventa in preventaGrouped)
        //    {
        //        var rows = response.Data.Items.Where(x => x.EmployeeId == preventa.Key).ToList();

        //        var preventaAprovedEmployee = preventaApprovedGrouped.FirstOrDefault(x => x.Key == preventa.Key);

        //        var count = preventa.Count();

        //        decimal hoursLoaded = 0;
        //        decimal hoursApproved = 0;
        //        decimal totalLeftLoaded = 0;
        //        decimal totalLeftApproved = 0;

        //        if (count > 0)
        //        {
        //            hoursLoaded = preventa.Sum(x => x) / count;
        //            totalLeftLoaded = preventa.Sum(x => x);

        //            if (preventaAprovedEmployee != null)
        //            {
        //                hoursApproved = preventaAprovedEmployee.Sum(x => x) / count;
        //                totalLeftApproved = preventaAprovedEmployee.Sum(x => x);
        //            }
        //        }

        //        foreach (var row in rows)
        //        {
        //            if (row.AnalyticId != 146 && row.AnalyticId != 166 && row.AnalyticId != 167)
        //            {
        //                if (totalLeftLoaded > 0)
        //                {
        //                    row.HoursLoaded += hoursLoaded;
        //                    totalLeftLoaded -= hoursLoaded;
        //                }

        //                if (totalLeftApproved > 0)
        //                {
        //                    row.HoursApproved += hoursApproved;
        //                    totalLeftApproved -= hoursApproved;
        //                }
        //            }
        //        }

        //        var toRemove = rows.Where(x => x.AnalyticId == 146 || x.AnalyticId == 166 || x.AnalyticId == 167)
        //            .ToList();

        //        foreach (var item in toRemove)
        //        {
        //            response.Data.Items.Remove(item);
        //        }
        //    }
        //}

        private void FillUnassigned(Response<WorkTimeReportModel> response, DateTime startDate, DateTime endDate, int employeeId)
        {
            var employeesUnassigned = unitOfWork.EmployeeRepository.GetUnassignedBetweenDays(startDate, endDate, employeeId);

            foreach (var employee in employeesUnassigned)
            {
                var employeeMissAllocation =
                    response.Data.EmployeesAllocationResume.SingleOrDefault(x => x.EmployeeId == employee.Id);

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

        private void CalculateEmployeesAllocationResume(Response<WorkTimeReportModel> response, Allocation allocation,
            ReportParams parameters, WorkTimeReportModelItem model)
        {
            if (allocation.StartDate.Year == parameters.EndYear && allocation.StartDate.Month == parameters.EndMonth)
            {
                model.AllocationId = allocation.Id;
            }

            var employeeMissAllocation =
                response.Data.EmployeesAllocationResume.SingleOrDefault(x => x.EmployeeId == allocation.EmployeeId);

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
                    itemToAdd.LastPercentage += allocation.Percentage == -1 ? 0 : allocation.Percentage;
                }
                else
                {
                    itemToAdd.CurrentMonth = allocation.StartDate.Month;
                    itemToAdd.CurrentMonthDescription = DatesHelper.GetDateDescription(allocation.StartDate);
                    itemToAdd.CurrentPercentage += allocation.Percentage == -1 ? 0 : allocation.Percentage;
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
                    employeeMissAllocation.LastPercentage += allocation.Percentage == -1 ? 0 : allocation.Percentage;
                }
                else
                {
                    employeeMissAllocation.CurrentMonth = allocation.StartDate.Month;
                    employeeMissAllocation.CurrentMonthDescription =
                        DatesHelper.GetDateDescription(allocation.StartDate);
                    employeeMissAllocation.CurrentPercentage += allocation.Percentage == -1 ? 0 : allocation.Percentage;
                }
            }
        }

        private void CalculateRealPercentage(Response<WorkTimeReportModel> response)
        {
            foreach (var item in response.Data.Items)
            {
                var allHoursMustLoaded = response.Data.Items.FirstOrDefault(x => x.EmployeeId == item.EmployeeId)
                    .AllHoursMustLoad;

                if (allHoursMustLoaded == 0) continue;

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
                        var totalRealPercentage = response.Data.Items.Where(x => x.EmployeeId == item.EmployeeId).Sum(x => x.RealPercentage);

                        var realPercentage = (percentageWithoutRound * (item.HoursApproved * 100 / item.HoursMustLoad)) / 100;
                        realPercentage = Convert.ToDecimal($"{realPercentage:0.00}");

                        var diff = (totalRealPercentage + realPercentage) - 100;

                        if (diff > 0)
                        {
                            realPercentage -= diff;
                        }
                        else
                        {
                            if (diff == (decimal)-0.01)
                            {
                                realPercentage += (decimal)0.01;
                            }
                        }

                        item.RealPercentage = realPercentage;

                        if (item.RealPercentage > 100) item.RealPercentage = 100;
                    }
                }
            }
        }

        private void SaveTigerTxt(Response<WorkTimeReportModel> response, List<TigerReportItem> tigerReport, DateTime date)
        {
            var i = 1;
            foreach (var item in response.Data.Items)
            {
                var resource = response.Data.Items.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                var sumAllHoursApproved = response.Data.Items.Where(x => x.EmployeeId == item.EmployeeId)
                    .Sum(x => x.HoursApproved);

                if (item.Facturability > 0)
                {
                    if (workTimeReportByHours)
                    {
                        if (sumAllHoursApproved >= resource.AllHoursMustLoad) item.HoursLoadedSuccesfully = true;
                    }
                    else
                    {
                        if (response.Data.Items.Where(x => x.EmployeeId == item.EmployeeId)
                                .Sum(x => x.AllocationPercentage) == 100)
                        {
                            item.Result = true;
                            item.HoursLoadedSuccesfully = true;
                        }
                        else
                        {
                            item.Result = false;
                            item.HoursLoadedSuccesfully = false;
                        }
                    }
                }
                else
                {
                    item.HoursLoadedSuccesfully = true;
                }

                if (workTimeReportByHours)
                {
                    if (item.RealPercentage <= 0) continue;
                    
                    var tigerItem = new TigerReportItem(item.EmployeeNumber, item.RealPercentage, item.CostCenter,
                        item.Activity, item.Title)
                    {
                        Id = i,
                        AllocationId = item.AllocationId,
                        Date = date,
                        AnalyticId = item.AnalyticId,
                        EmployeeId = item.EmployeeId
                    };
                    i++;

                    tigerReport.Add(tigerItem);
                }
                else
                {
                    if (item.AllocationPercentage <= 0) continue;

                    var tigerItem = new TigerReportItem(item.EmployeeNumber, item.AllocationPercentage, item.CostCenter,
                        item.Activity, item.Title)
                    {
                        Id = i,
                        AllocationId = item.AllocationId,
                        Date = date,
                        AnalyticId = item.AnalyticId,
                        EmployeeId = item.EmployeeId
                    };
                    i++;

                    tigerReport.Add(tigerItem);
                }
            }
        }

        private Tuple<decimal, decimal> CalculateHoursToLoad(Allocation allocation, DateTime startDate,
            DateTime endDate, IList<Holiday> holidays)
        {
            var businessDays = 0;
            var allBusinessDays = 0;

            while (startDate.Date <= endDate.Date)
            {
                if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday &&
                    holidays.All(x => x.Date.Date != startDate.Date) &&
                    allocation.Employee.StartDate.Date <= startDate.Date)
                {
                    if (allocation.Employee.EndDate.HasValue)
                    {
                        if (allocation.Employee.EndDate.Value.Date > startDate.Date)
                        {
                            if (allocation.StartDate.Month == startDate.Month) businessDays++;

                            allBusinessDays++;
                        }
                    }
                    else
                    {
                        if (allocation.StartDate.Month == startDate.Month) businessDays++;

                        allBusinessDays++;
                    }
                }

                //if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday && holidays.All(x => x.Date.Date != startDate.Date))
                //    allBusinessDays++;

                startDate = startDate.AddDays(1);
            }

            var hoursMustLoad =
                Math.Round((businessDays * allocation.Employee.BusinessHours * allocation.Percentage) / 100);
            var allHoursMustLoad = allBusinessDays * allocation.Employee.BusinessHours;

            return new Tuple<decimal, decimal>(hoursMustLoad, allHoursMustLoad);
        }
    }
}