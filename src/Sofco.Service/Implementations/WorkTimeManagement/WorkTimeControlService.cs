using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Managers;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.Domain;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.WorkTimeManagement
{
    public class WorkTimeControlService : IWorkTimeControlService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly IWorkTimeResumeManager workTimeResumeManager;

        private readonly IRoleManager roleManager;

        private readonly IMapper mapper;

        public WorkTimeControlService(IUnitOfWork unitOfWork, IUserData userData, IWorkTimeResumeManager workTimeResumeManager, IMapper mapper, IRoleManager roleManager)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.workTimeResumeManager = workTimeResumeManager;
            this.mapper = mapper;
            this.roleManager = roleManager;
        }

        public Response<WorkTimeControlModel> Get(WorkTimeControlParams parameters)
        {
            var result = new Response<WorkTimeControlModel>();

            var startDate = parameters.StartDate;

            var endDate = parameters.EndDate;

            var workTimes = unitOfWork.WorkTimeRepository.GetByAnalyticIds(startDate, endDate, GetAnalyticIds(parameters.AnalyticId));

            var models = workTimes.Select(x => new WorkTimeCalendarModel(x)).ToList();

            var resumeModel = workTimeResumeManager.GetResume(models, startDate, endDate);

            var resources = GetResources(workTimes.ToList(), startDate, endDate);

            result.Data = new WorkTimeControlModel
            {
                Resume = resumeModel,
                Resources = resources
            };

            resumeModel.BusinessHours = resources.Sum(s => s.BusinessHours);

            resumeModel.HoursPending = resources.Sum(s => s.PendingHours);

            resumeModel.HoursApproved = resources.Sum(s => s.ApprovedHours);

            return result;
        }

        private List<WorkTimeControlResourceModel> GetResources(List<WorkTime> workTimes, DateTime startDate, DateTime endDate)
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

                var models = list.Select(x => new WorkTimeCalendarModel(x)).ToList();

                var resume = workTimeResumeManager.GetResume(models, startDate, endDate);

                var allocations = unitOfWork.AllocationRepository.GetAllocationsLiteBetweenDaysForWorkTimeControl(model.EmployeeId, startDate, endDate);

                var allocationAnalytic = allocations?.FirstOrDefault(s => s.AnalyticId == model.AnalyticId);

                if (allocationAnalytic == null) continue;

                resource.BusinessHours = resume.BusinessHours * allocationAnalytic.Percentage / 100;
                resource.ApprovedHours = item.Value.Where(x => x.Status == WorkTimeStatus.Approved).Sum(x => x.Hours);
                resource.LicenseHours = item.Value.Where(x => x.Status == WorkTimeStatus.License).Sum(x => x.Hours);
                resource.SentHours = item.Value.Where(x => x.Status == WorkTimeStatus.Sent).Sum(x => x.Hours);
                resource.DraftHours = item.Value.Where(x => x.Status == WorkTimeStatus.Draft).Sum(x => x.Hours);

                resource.PendingHours = resource.BusinessHours - resource.ApprovedHours - resource.LicenseHours - resource.SentHours - resource.DraftHours;

                resource.AllocationPercentage = allocationAnalytic.Percentage;
                resource.Details = Translate(list.OrderBy(s => s.Date).ToList());
                result.Add(resource);
            }

            return result;
        }

        private List<int> GetAnalyticIds(int? analyticId)
        {
            if (analyticId.HasValue) return new List<int> {analyticId.Value};

            if(roleManager.IsDirector()) return unitOfWork.AnalyticRepository.GetAllOpenReadOnly().Select(s => s.Id).ToList();

            var currentUser = userData.GetCurrentUser();

            return unitOfWork.AnalyticRepository.GetAnalyticLiteByManagerId(currentUser.Id).Select(s => s.Id).ToList();
        }

        private List<WorkTimeControlResourceDetailModel> Translate(List<WorkTime> workTimes)
        {
            var categoriyIds = workTimes.Select(s => s.Task.CategoryId).Distinct().ToList();

            var categories = unitOfWork.CategoryRepository.GetByIds(categoriyIds);

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
    }
}
