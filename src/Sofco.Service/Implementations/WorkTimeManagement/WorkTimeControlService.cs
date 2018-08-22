using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Managers;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.Domain;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.WorkTimeManagement
{
    public class WorkTimeControlService : IWorkTimeControlService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly IWorkTimeResumeManager workTimeResumeManager;

        public WorkTimeControlService(IUnitOfWork unitOfWork, IUserData userData, IWorkTimeResumeManager workTimeResumeManager)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.workTimeResumeManager = workTimeResumeManager;
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
                Resources = GetResources()
            };

            return result;
        }

        private List<WorkTimeControlResourceModel> GetResources()
        {
            return new List<WorkTimeControlResourceModel>
            {
                new WorkTimeControlResourceModel
                {
                    Analytic = "111-5444",
                    EmployeeNumber = "6900",
                    EmployeeName = "Luan Oliveira"
                }
            };
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
    }
}
