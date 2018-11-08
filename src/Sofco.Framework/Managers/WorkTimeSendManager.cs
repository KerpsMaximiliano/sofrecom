using System;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Managers;
using Sofco.Core.Validations;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Managers
{
    public class WorkTimeSendManager : IWorkTimeSendManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly IEmployeeData employeeData;

        private readonly ILogMailer<WorkTimeSendManager> logger;

        private readonly IWorkTimeSendMailManager workTimeSendMailManager;

        private readonly IWorkTimeValidation workTimeValidation;

        public WorkTimeSendManager(IWorkTimeSendMailManager workTimeSendMailManager, IUserData userData, IUnitOfWork unitOfWork, IEmployeeData employeeData, ILogMailer<WorkTimeSendManager> logger, IWorkTimeValidation workTimeValidation)
        {
            this.workTimeSendMailManager = workTimeSendMailManager;
            this.userData = userData;
            this.unitOfWork = unitOfWork;
            this.employeeData = employeeData;
            this.logger = logger;
            this.workTimeValidation = workTimeValidation;
        }

        public Response Send()
        {
            var response = Validate();
            if (response.HasErrors())
            {
                return response;
            }

            var isManager = unitOfWork.UserRepository.HasManagerGroup(userData.GetCurrentUser().UserName);

            try
            {
                var currentEmployeeId = employeeData.GetCurrentEmployee().Id;

                if (isManager)
                {
                    unitOfWork.WorkTimeRepository.SendManagerHours(currentEmployeeId);
                }
                else
                {
                    unitOfWork.WorkTimeRepository.SendHours(currentEmployeeId);
                }

                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.SentSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            if (!response.HasErrors() && !isManager)
            {
                workTimeSendMailManager.SendEmail();
            }

            return response;
        }

        private Response Validate()
        {
            var response = new Response();

            var pendingWorkTimes =
                unitOfWork.WorkTimeRepository.GetWorkTimeDraftByEmployeeId(employeeData.GetCurrentEmployee().Id);

            if (!pendingWorkTimes.Any())
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.NoPendingHours);
                return response;
            }

            workTimeValidation.ValidateAllocations(response, pendingWorkTimes);

            return response;
        }
    }
}
