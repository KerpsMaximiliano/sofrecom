using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Managers;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;
using Sofco.Framework.MailData;
using Sofco.Framework.ValidationHelpers.WorkTimeManagement;
using Sofco.Resources.Mails;

namespace Sofco.Framework.Managers
{
    public class WorkTimeRejectManager : IWorkTimeRejectManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly ILogMailer<WorkTimeRejectManager> logger;

        private readonly EmailConfig emailConfig;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        public WorkTimeRejectManager(IUnitOfWork unitOfWork, IUserData userData, ILogMailer<WorkTimeRejectManager> logger, IOptions<EmailConfig> emailOptions, IMailBuilder mailBuilder, IMailSender mailSender)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.logger = logger;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
            emailConfig = emailOptions.Value;
        }

        public Response Reject(int workTimeId, string comments)
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

                SendEmail(workTime);

                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.RejectedSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private IMailData GetEmailData(WorkTime workTime)
        {
            var subject = string.Format(MailSubjectResource.WorkTimeReject, workTime.Employee.Name);

            var body = string.Format(MailMessageResource.WorkTimeRejectHours,
                $"{workTime.Analytic.Title} - {workTime.Analytic.Name}",
                workTime.Task.Description,
                workTime.Date.ToString("dd/MM/yyyy"),
                workTime.Hours,
                workTime.ApprovalComment,
                workTime.Employee.Name);

            var recipients = new List<string>
            {
                workTime.Employee.Email,
                workTime.Analytic.Manager.Email
            };

            var approverDelegates =
                unitOfWork.UserApproverRepository.GetApproverByEmployeeIdAndAnalyticId(
                    workTime.EmployeeId,
                    workTime.AnalyticId,
                    UserApproverType.WorkTime);

            recipients.AddRange(approverDelegates.Select(s => s.Email));

            return new MailDefaultData
            {
                Title = subject,
                Message = body,
                Recipients = recipients
            };
        }

        private void SendEmail(WorkTime workTime)
        {
            try
            {
                workTime.Employee = unitOfWork.EmployeeRepository.GetById(workTime.EmployeeId);
                workTime.Analytic = unitOfWork.AnalyticRepository.GetById(workTime.AnalyticId);
                workTime.Task = unitOfWork.TaskRepository.GetById(workTime.TaskId);

                var data = GetEmailData(workTime);

                var email = mailBuilder.GetEmail(data);

                mailSender.Send(email);
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }
    }
}
