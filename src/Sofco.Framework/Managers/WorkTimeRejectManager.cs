using System;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Managers;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;
using Sofco.Framework.ValidationHelpers.WorkTimeManagement;

namespace Sofco.Framework.Managers
{
    public class WorkTimeRejectManager : IWorkTimeRejectManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly IWorkTimeRejectMailManager workTimeRejectMailManager;

        private readonly ILogMailer<WorkTimeRejectManager> logger;

        public WorkTimeRejectManager(IUnitOfWork unitOfWork, IUserData userData, 
            ILogMailer<WorkTimeRejectManager> logger, IWorkTimeRejectMailManager workTimeRejectMailManager)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.logger = logger;
            this.workTimeRejectMailManager = workTimeRejectMailManager;
        }

        public Response Reject(int workTimeId, string comments, bool massive)
        {
            var response = new Response();

            var workTime = unitOfWork.WorkTimeRepository.GetSingle(x => x.Id == workTimeId);

            WorkTimeValidationHandler.ValidateApproveOrReject(workTime, response);

            if (response.HasErrors()) return response;

            try
            {
                workTime.Status = WorkTimeStatus.Rejected;
                workTime.ApprovalComment = comments;
                workTime.ApprovalUserId = userData.GetCurrentUser().Id;

                unitOfWork.WorkTimeRepository.UpdateStatus(workTime);
                unitOfWork.WorkTimeRepository.UpdateApprovalComment(workTime);
                unitOfWork.Save();

                if(!massive) workTimeRejectMailManager.SendEmail(workTime);

                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.RejectedSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public void SendGeneralRejectMail(WorkTime workTime)
        {
            workTimeRejectMailManager.SendGeneraEmail(workTime);
        }
    }
}
