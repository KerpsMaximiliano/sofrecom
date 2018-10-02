using System;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Managers;
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

        public WorkTimeSendManager(IWorkTimeSendMailManager workTimeSendMailManager, IUserData userData, IUnitOfWork unitOfWork, IEmployeeData employeeData, ILogMailer<WorkTimeSendManager> logger)
        {
            this.workTimeSendMailManager = workTimeSendMailManager;
            this.userData = userData;
            this.unitOfWork = unitOfWork;
            this.employeeData = employeeData;
            this.logger = logger;
        }

        public Response Send()
        {
            var response = new Response();

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
    }
}
