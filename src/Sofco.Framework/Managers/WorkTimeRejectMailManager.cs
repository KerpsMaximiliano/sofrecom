using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Managers;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Framework.MailData;
using Sofco.Resources.Mails;

namespace Sofco.Framework.Managers
{
    public class WorkTimeRejectMailManager : IWorkTimeRejectMailManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ILogMailer<WorkTimeRejectMailManager> logger;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        public WorkTimeRejectMailManager(IUnitOfWork unitOfWork, 
            ILogMailer<WorkTimeRejectMailManager> logger,
            IMailBuilder mailBuilder, IMailSender mailSender)
        {
            this.unitOfWork = unitOfWork;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
            this.logger = logger;
        }

        public void SendEmail(WorkTime workTime)
        {
            try
            {
                workTime.Employee = workTime.Employee ?? unitOfWork.EmployeeRepository.GetById(workTime.EmployeeId);
                workTime.Analytic = workTime.Analytic ?? unitOfWork.AnalyticRepository.GetById(workTime.AnalyticId);
                workTime.Task = workTime.Task ?? unitOfWork.TaskRepository.GetById(workTime.TaskId);

                var data = GetEmailData(workTime);

                var email = mailBuilder.GetEmail(data);

                mailSender.Send(email);
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        public void SendGeneraEmail(WorkTime workTime)
        {
            if(workTime == null) return;
            
            try
            {
                workTime.Employee = workTime.Employee ?? unitOfWork.EmployeeRepository.GetById(workTime.EmployeeId);
                workTime.Analytic = workTime.Analytic ?? unitOfWork.AnalyticRepository.GetById(workTime.AnalyticId);

                var subject = string.Format(MailSubjectResource.WorkTimeReject, workTime.Employee.Name);

                var body = string.Format(MailMessageResource.WorkTimeGeneralRejectHours, $"{workTime.Analytic.Title} - {workTime.Analytic.Name}", workTime.Employee.Name);

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

                var maildata = new MailDefaultData
                {
                    Title = subject,
                    Message = body,
                    Recipients = recipients
                };

                var email = mailBuilder.GetEmail(maildata);

                mailSender.Send(email);
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
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
                workTime.Employee.Name,
                workTime.UserComment,
                workTime.Reference);

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
    }
}
