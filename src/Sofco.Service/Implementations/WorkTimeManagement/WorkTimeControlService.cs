﻿using AutoMapper;
using Sofco.Core.DAL;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.WorktimeManagement;
using Sofco.Core.FileManager;
using Sofco.Core.Managers;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.Service.Implementations.WorkTimeManagement
{
    public class WorkTimeControlService : IWorkTimeControlService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly IWorkTimeResumeManager workTimeResumeManager;

        private readonly IRoleManager roleManager;

        private readonly IMapper mapper;

        private readonly IWorktimeData worktimeData;

        private readonly IWorkTimeControlHoursFileManager workTimeControlHoursFileManager;

        public WorkTimeControlService(IUnitOfWork unitOfWork,
            IUserData userData,
            IWorkTimeResumeManager workTimeResumeManager,
            IMapper mapper,
            IRoleManager roleManager,
            IWorktimeData worktimeData,
            IWorkTimeControlHoursFileManager workTimeControlHoursFileManager)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.workTimeResumeManager = workTimeResumeManager;
            this.mapper = mapper;
            this.roleManager = roleManager;
            this.worktimeData = worktimeData;
            this.workTimeControlHoursFileManager = workTimeControlHoursFileManager;
        }

        public Response<WorkTimeControlModel> Get(WorkTimeControlParams parameters)
        {
            var result = new Response<WorkTimeControlModel>();

            SetStartEndDateParameters(parameters);

            var startDate = parameters.StartDate;

            var endDate = parameters.EndDate;

            var analyticIds = GetAnalyticIds(parameters.AnalyticId);

            var workTimes = unitOfWork.WorkTimeRepository
                .GetByAnalyticIds(startDate, endDate, analyticIds)
                .ToList();

            workTimes.AddRange(AddDelegatedData(parameters, workTimes));

            var daysoff = unitOfWork.HolidayRepository.Get(startDate.Year, startDate.Month);
            daysoff.AddRange(unitOfWork.HolidayRepository.Get(endDate.Year, endDate.Month));

            var models = workTimes.Select(x => new WorkTimeCalendarModel(x)).ToList();

            var resumeModel = workTimeResumeManager.GetResume(models, startDate, endDate);

            var resources = GetResources(workTimes.ToList(), startDate, endDate, daysoff);

            var allResources = unitOfWork.AnalyticRepository.GetResources(analyticIds, startDate, endDate);

            var currentUser = userData.GetCurrentUser();

            var resourcesAux = resources.ToList();

            foreach (var allResource in allResources)
            {
                foreach (var allocation in allResource.Allocations)
                {
                    if (parameters.AnalyticId != null && parameters.AnalyticId.Any() && !parameters.AnalyticId.Contains(allocation.AnalyticId)) continue;

                    if (resourcesAux.Any(x => x.Id == $"{allocation.AnalyticId}-{allocation.EmployeeId}")) continue;

                    if ((allocation.StartDate.Year == startDate.Year &&
                         allocation.StartDate.Month == startDate.Month) ||
                        (allocation.StartDate.Year == endDate.Year && allocation.StartDate.Month == endDate.Month))
                    {
                        var item = resources.SingleOrDefault(x => x.Id == $"{allocation.AnalyticId}-{allocation.EmployeeId}");

                        if (item == null)
                        {
                            var hours = CalculateHoursToLoad(allocation, startDate, endDate, daysoff);

                            resources.Add(new WorkTimeControlResourceModel
                            {
                                Id = $"{allocation.AnalyticId}-{allocation.EmployeeId}",
                                ApprovedHours = 0,
                                BusinessHours = hours,
                                DraftHours = 0,
                                EmployeeName = allResource.Name,
                                EmployeeNumber = allResource.EmployeeNumber,
                                LicenseHours = 0,
                                PendingHours = hours,
                                RejectHours = 0,
                                SentHours = 0,
                                DetailCount = 0,
                                Details = new List<WorkTimeControlResourceDetailModel>(),
                                AllocationPercentage = allocation.Percentage,
                                Analytic = allocation.Analytic.Title
                            });
                        }
                        else
                        {
                            var hours = CalculateHoursToLoad(allocation, startDate, endDate, daysoff);

                            item.BusinessHours += hours;
                            item.PendingHours += hours;
                        }
                    }
                }
            }

            resumeModel.BusinessHours = resources.Sum(s => s.BusinessHours);
            resumeModel.HoursPending = resources.Sum(s => s.PendingHours);

            worktimeData.ClearControlHoursReportKey(currentUser.UserName);
            worktimeData.SaveControlHoursReport(resources, currentUser.UserName);

            result.Data = new WorkTimeControlModel
            {
                Resume = resumeModel,
                Resources = resources
            };

            return result;
        }
        
        private List<WorkTimeControlResourceModel> GetResources(List<WorkTime> workTimes, DateTime startDate,
            DateTime endDate, List<Holiday> daysoff)
        {
            var grouped = new Dictionary<string, List<WorkTime>>();
            foreach (var workTime in workTimes)
            {
                var key = workTime.Analytic.Title + workTime.Employee.EmployeeNumber;
                if (grouped.ContainsKey(key))
                {
                    grouped[key].Add(workTime);
                }
                else
                {
                    grouped.Add(key, new List<WorkTime> { workTime });
                }
            }

            var result = new List<WorkTimeControlResourceModel>();

            foreach (var item in grouped)
            {
                var list = item.Value;
                var model = list.First();

                var resource = Translate(model);

                //var models = list.Select(x => new WorkTimeCalendarModel(x)).ToList();

                //var resume = workTimeResumeManager.GetResume(models, startDate, endDate);

                var allocations = unitOfWork.AllocationRepository.GetAllocationsLiteBetweenDaysForWorkTimeControl(model.EmployeeId, startDate, endDate);

                var allocationAnalytic = allocations?.Where(s => s.AnalyticId == model.AnalyticId).ToList();

                if (allocationAnalytic != null)
                {
                    resource.BusinessHours = CalculateHoursToLoad(allocationAnalytic, startDate, endDate, daysoff);
                    //resource.BusinessHours = resume.BusinessHours * allocationAnalytic.Percentage / 100;
                    //resource.AllocationPercentage = allocationAnalytic.Percentage;
                }

                resource.ApprovedHours = item.Value.Where(x => x.Status == WorkTimeStatus.Approved).Sum(x => x.Hours);
                resource.LicenseHours = item.Value.Where(x => x.Status == WorkTimeStatus.License).Sum(x => x.Hours);
                resource.SentHours = item.Value.Where(x => x.Status == WorkTimeStatus.Sent).Sum(x => x.Hours);
                resource.DraftHours = item.Value.Where(x => x.Status == WorkTimeStatus.Draft).Sum(x => x.Hours);
                resource.RejectHours = item.Value.Where(x => x.Status == WorkTimeStatus.Rejected).Sum(x => x.Hours);
                resource.PendingHours = resource.BusinessHours - resource.ApprovedHours - resource.LicenseHours - resource.SentHours - resource.DraftHours;

                if (resource.PendingHours < 0) resource.PendingHours = 0;

                var details = list.OrderBy(s => s.Date).ToList();
                resource.Details = Translate(details);
                resource.DetailCount = details.Count;
                result.Add(resource);
            }

            return result;
        }

        private List<int> GetAnalyticIds(IList<int> analyticId)
        {
            var analyticsByManagers = roleManager.HasFullAccess()
                ? unitOfWork.AnalyticRepository.GetAllOpenAnalyticLite()
                : GetAnalyticByManagerAndDelegated();

            var availableAnalyticIds = analyticsByManagers.Select(x => x.Id).ToList();

            if (analyticId != null && analyticId.Any())
            {
                var result = new List<int>();

                foreach (var id in analyticId)
                {
                    if (availableAnalyticIds.Contains(id))
                    {
                        result.Add(id);
                    }    
                }

                return result;
            }
            else
            {
                return availableAnalyticIds;
            }
        }
        
        private List<WorkTimeControlResourceDetailModel> Translate(List<WorkTime> workTimes)
        {
            var categoryIds = workTimes.Select(s => s.Task.CategoryId).Distinct().ToList();

            var categories = unitOfWork.CategoryRepository.GetByIds(categoryIds);

            foreach (var workTime in workTimes)
            {
                workTime.Task.Category = categories.Single(s => s.Id == workTime.Task.CategoryId);
            }

            return mapper.Map<List<WorkTime>, List<WorkTimeControlResourceDetailModel>>(workTimes);
        }

        private WorkTimeControlResourceModel Translate(WorkTime workTime)
        {
            return mapper.Map<WorkTime, WorkTimeControlResourceModel>(workTime);
        }

        private void SetStartEndDateParameters(WorkTimeControlParams parameters)
        {
            if (!parameters.CloseMonthId.HasValue)
            {
                var currentCloseDates = unitOfWork.CloseDateRepository.GetBeforeCurrentAndNext();

                var period = currentCloseDates.GetPeriodIncludeDays();
                parameters.StartDate = period.Item2.Date;
                parameters.EndDate = period.Item1.Date;
                return;
            }

            var closeDates = unitOfWork.CloseDateRepository.GetBeforeAndCurrent(parameters.CloseMonthId.Value);
            parameters.StartDate = new DateTime(closeDates.Item2.Year, closeDates.Item2.Month, closeDates.Item2.Day + 1);
            parameters.EndDate = new DateTime(closeDates.Item1.Year, closeDates.Item1.Month, closeDates.Item1.Day);
        }

        private ICollection<AnalyticLiteModel> GetAnalyticByManagerAndDelegated()
        {
            var currentUser = userData.GetCurrentUser();

            var analytics = unitOfWork.AnalyticRepository
                .GetAnalyticLiteByManagerId(currentUser.Id)
                .ToList();

            var delegations =
                unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(currentUser.Id, DelegationType.WorkTime);

            if (!delegations.Any()) return analytics;

            var delegatedAnalytics =
                unitOfWork.AnalyticRepository
                    .GetAnalyticLiteByIds(delegations.Select(s => s.AnalyticSourceId.GetValueOrDefault())
                    .ToList());

            analytics.AddRange(delegatedAnalytics);

            return analytics;
        }

        private List<WorkTime> AddDelegatedData(WorkTimeControlParams parameter, List<WorkTime> workTimes)
        {
            var result = new List<WorkTime>();

            var currentUser = userData.GetCurrentUser();

            var delegations =
                unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(currentUser.Id, DelegationType.WorkTime);

            if (parameter.AnalyticId != null && parameter.AnalyticId.Any())
            {
                delegations = delegations
                    .Where(s => parameter.AnalyticId.Contains(s.AnalyticSourceId.GetValueOrDefault()))
                    .ToList();
            }

            if (!delegations.Any()) return result;

            var employeeIds = delegations.Select(s => s.EmployeeSourceId.GetValueOrDefault()).ToList();

            var alreadyLoadedEmployeeIds = workTimes.Select(s => s.EmployeeId).Distinct();

            employeeIds.RemoveAll(s => alreadyLoadedEmployeeIds.Contains(s));

            result = unitOfWork
                .WorkTimeRepository
                .GetByEmployeeIds(parameter.StartDate, parameter.EndDate, employeeIds)
                .ToList();

            return result;
        }

        public Response<byte[]> ExportControlHoursReport()
        {
            var response = new Response<byte[]>();

            var currentuser = userData.GetCurrentUser();

            var list = worktimeData.GetAllControlHoursReport(currentuser.UserName);

            if (!list.Any())
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.ControlHoursEmpty);
                return response;
            }

            var excel = workTimeControlHoursFileManager.CreateExcel(list);

            response.Data = excel.GetAsByteArray();

            return response;
        }

        public IList<Option> GetAnalytics()
        {
            if (roleManager.IsPmo() || roleManager.IsRrhh() || roleManager.IsCdg())
            {
                var analytics = unitOfWork.AnalyticRepository.GetAllOpenAnalyticLite();

                return analytics.Select(x => new Option { Id = x.Id, Text = $"{x.Title} - {x.Name}" }).ToList();
            }
            else 
            {
                var currentUser = userData.GetCurrentUser();
                var analyticsByManagers = unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(currentUser.Id);
                var analyticsByDirectors = unitOfWork.AnalyticRepository.GetByDirectorId(currentUser.Id);

                var delegations = unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(currentUser.Id, DelegationType.WorkTime);

                var list = analyticsByManagers.Select(x => new Option { Id = x.Id, Text = $"{x.Title} - {x.Name}" }).ToList();

                foreach (var analytic in analyticsByDirectors)
                {
                    if (list.All(x => x.Id != analytic.Id))
                    {
                        list.Add(new Option { Id = analytic.Id, Text = $"{analytic.Title} - {analytic.Name}" });
                    }
                }

                foreach (var delegation in delegations)
                {
                    if (list.All(x => x.Id != delegation.AnalyticSourceId))
                    {
                        var analytic = unitOfWork.AnalyticRepository.Get(delegation.AnalyticSourceId.GetValueOrDefault());

                        list.Add(new Option { Id = analytic.Id, Text = $"{analytic.Title} - {analytic.Name}" });
                    }
                }

                return list;
            }
        }

        private decimal CalculateHoursToLoad(IList<Allocation> allocations, DateTime startDate, DateTime endDate, IList<Holiday> holidays)
        {
            decimal hours = 0;

            while (startDate.Date <= endDate.Date)
            {
                foreach (var allocation in allocations)
                {
                    if (allocation.StartDate.Month == startDate.Month)
                    {
                        if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday &&
                            holidays.All(x => x.Date.Date != startDate.Date))
                        {
                            hours += (allocation.Employee.BusinessHours * allocation.Percentage) / 100;
                        }
                    }
                }

                startDate = startDate.AddDays(1);
            }

            return Math.Round(hours);
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

            var hoursMustLoad = Math.Round((businessDays * allocation.Employee.BusinessHours * allocation.Percentage) / 100);

            return hoursMustLoad;
        }
    }
}
