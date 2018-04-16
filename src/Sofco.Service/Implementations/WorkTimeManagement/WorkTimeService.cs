using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.Framework.ValidationHelpers.WorkTimeManagement;
using Sofco.Model.Models.WorkTimeManagement;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.WorkTimeManagement
{
    public class WorkTimeService : IWorkTimeService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly ISessionManager sessionManager;

        private readonly ILogMailer<WorkTimeService> logger;

        public WorkTimeService(ISessionManager sessionManager, ILogMailer<WorkTimeService> logger, IUnitOfWork unitOfWork, IUserData userData)
        {
            this.sessionManager = sessionManager;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.logger = logger;
        }

        public IList<WorkTimeModel> Get(DateTime date)
        {
            if(date == DateTime.MinValue) return new List<WorkTimeModel>();

            try
            {
                var userName = sessionManager.GetUserName();
                var currentUser = userData.GetByUserName(userName);

                var startDate = new DateTime(date.Year, date.Month, 1);
                var endDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

                var workTimes = unitOfWork.WorkTimeRepository.Get(startDate.Date, endDate.Date, currentUser.Id);

                var list = workTimes.Select(x => new WorkTimeModel(x)).ToList();

                return list;
            }
            catch (Exception e)
            {
                logger.LogError(e);
                return new List<WorkTimeModel>();
            }
        }

        public Response<WorkTimeAddModel> Add(WorkTimeAddModel model)
        {
            var response = new Response<WorkTimeAddModel>();

            WorkTimeValidationHandler.ValidateEmployee(response, unitOfWork, model);
            WorkTimeValidationHandler.ValidateAnalytic(response, unitOfWork, model);
            WorkTimeValidationHandler.ValidateUser(response, unitOfWork, model);
            WorkTimeValidationHandler.ValidateDate(response, unitOfWork, model);
            WorkTimeValidationHandler.ValidateHours(response, unitOfWork, model);
            WorkTimeValidationHandler.ValidateTask(response, unitOfWork, model);

            if (response.HasErrors()) return response;

            try
            {
                var workTime = model.CreateDomain();

                //todo: traer el aprobador del empleado
                workTime.ApprovalUserId = 1;

                unitOfWork.WorkTimeRepository.Insert(workTime);
                unitOfWork.Save();

                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.AddSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }
    }
}
