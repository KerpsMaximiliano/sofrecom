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

        private readonly IMapper mapper;

        public WorkTimeControlService(IUnitOfWork unitOfWork, IUserData userData, IWorkTimeResumeManager workTimeResumeManager, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.workTimeResumeManager = workTimeResumeManager;
            this.mapper = mapper;
        }

        public Response<WorkTimeControlModel> Get(WorkTimeControlParams parameters)
        {
            var result = new Response<WorkTimeControlModel>();

            var closeDay = GetCloseDay();

            var endDate = new DateTime(parameters.Year, parameters.Month, closeDay);

            var startDate = endDate.AddMonths(-1);

            var currentUser = userData.GetCurrentUser();

            var workTimes = parameters.ServiceId.HasValue
                ? unitOfWork.WorkTimeRepository.Get(startDate, endDate, currentUser.Id, GetAnalyticId(parameters.ServiceId))
                : unitOfWork.WorkTimeRepository.Get(startDate, endDate, currentUser.Id);

            var models = workTimes.Select(x => new WorkTimeCalendarModel(x)).ToList();

            var resumeModel = workTimeResumeManager.GetResume(models, startDate, endDate);

            result.Data = new WorkTimeControlModel
            {
                Resume = resumeModel,
                Resources = GetResources(workTimes.ToList(), startDate, endDate)
            };

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
                    grouped.Add(key, new List<WorkTime> { workTime});
                }
            }

            var result = new List<WorkTimeControlResourceModel>();

            foreach (var item in grouped)
            {
                var list = item.Value;
                var model = list.First();

                var resource = new WorkTimeControlResourceModel
                {
                    Analytic = model.Analytic.Title,
                    EmployeeName = model.Employee.Name,
                    EmployeeNumber = model.Employee.EmployeeNumber
                };

                var models = list.Select(x => new WorkTimeCalendarModel(x)).ToList();

                var resume = workTimeResumeManager.GetResume(models, startDate, endDate);

                var allocations = unitOfWork.AllocationRepository.GetAllocationsLiteBetweenDays(model.EmployeeId, startDate, endDate);

                var allocationAnalytic = allocations?.FirstOrDefault(s => s.AnalyticId == model.AnalyticId);

                if (allocationAnalytic == null) continue;

                resource.BusinessHours = resume.BusinessHours * allocationAnalytic.Percentage / 100;
                resource.RegisteredHours = resume.HoursApproved;
                resource.PendingHours = resume.HoursPending;
                resource.LicenseHours = resume.HoursWithLicense;
                resource.Details = Translate(workTimes);
                result.Add(resource);
            }

            return result;
        }

        private int GetAnalyticId(Guid? serviceId)
        {
            if (!serviceId.HasValue) return 0;

            var analytic = unitOfWork.AnalyticRepository
                .GetByService(serviceId.Value.ToString());

            return analytic?.Id ?? 0;
        }

        private int GetCloseDay()
        {
            var closeMonthSetting = unitOfWork.SettingRepository.GetByKey(SettingConstant.CloseMonthKey);

            return int.Parse(closeMonthSetting.Value);
        }

        private List<WorkTimeControlResourceDetailModel> Translate(List<WorkTime> workTimes)
        {
            return mapper.Map<List<WorkTime>, List<WorkTimeControlResourceDetailModel>>(workTimes);
        }
    }
}
