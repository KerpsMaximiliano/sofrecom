using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Managers;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Framework.MailData;
using Sofco.Resources.Mails;

namespace Sofco.Framework.Managers
{
    public class WorkTimeSendMailManager : IWorkTimeSendMailManager
    {
        private readonly IMailSender mailSender;

        private readonly IEmployeeData employeeData;

        private readonly IUnitOfWork unitOfWork;

        private readonly ILogMailer<WorkTimeSendMailManager> logger;

        public WorkTimeSendMailManager(IMailSender mailSender,
            IEmployeeData employeeData, IUnitOfWork unitOfWork,
            ILogMailer<WorkTimeSendMailManager> logger)
        {
            this.mailSender = mailSender;
            this.employeeData = employeeData;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public void SendEmail()
        {
            try
            {
                var mails = GetMails();

                if(!mails.Any()) return;

                mailSender.Send(mails);
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        protected List<IMailData> GetMails()
        {
            var currentEmployee = employeeData.GetCurrentEmployee();

            var closeDates = unitOfWork.CloseDateRepository.GetBeforeCurrentAndNext();

            var period = closeDates.GetPeriodExcludeDays();

            var dateFrom = period.Item1;

            var dateTo = period.Item2;

            var analytics = unitOfWork.AnalyticRepository.GetByAllocations(currentEmployee.Id, dateFrom, dateTo);

            if (!analytics.Any()) return new List<IMailData>();

            var workTimes = unitOfWork.WorkTimeRepository.GetWorkTimePendingHoursByEmployeeId(currentEmployee.Id);

            var mails = new List<IMailData>();

            foreach (var analytic in analytics)
            {
                var workTimesByAnalytic = workTimes.Where(s => s.AnalyticId == analytic.Id).ToList();

                if(!workTimesByAnalytic.Any()) continue;

                mails.Add(GetMailData(currentEmployee, workTimesByAnalytic, analytic));
            }

            return mails;
        }

        private IMailData GetMailData(Employee currentEmployee, List<WorkTime> workTimes, Analytic analytic)
        {
            var subject = string.Format(MailSubjectResource.WorkTimeSendHours, currentEmployee.Name);

            var body = string.Format(MailMessageResource.WorkTimeSendHours,
                currentEmployee.Name,
                BuildMailWorkTimes(workTimes));

            return new MailDefaultData
            {
                Title = subject,
                Message = body,
                Recipients = GetRecipients(currentEmployee, analytic)
            };
        }

        private string BuildMailWorkTimes(List<WorkTime> workTimes)
        {
            var text = new StringBuilder();

            foreach (var item in workTimes.OrderBy(s => s.Date))
            {
                text.AppendFormat("&nbsp; {0:dd/MM/yyyy} - {1}: {2} - {3}: {4} - {5} {6}",
                    item.Date, MailCommonResource.Hours, item.Hours,
                    MailCommonResource.Analytic, item.Analytic.Title, item.Analytic.Name,
                    MailDataConstant.Enter);
            }

            return text.ToString();
        }

        private List<string> GetRecipients(Employee currentEmployee, Analytic analytic)
        {
            var mails = new List<string> { analytic.Manager.Email };

            var delegates = unitOfWork.UserApproverRepository.GetApproverByEmployeeIdAndAnalyticId(
                currentEmployee.Id,
                analytic.Id,
                UserApproverType.WorkTime);

            mails.AddRange(delegates.Select(x => x.Email));

            return mails;
        }
    }
}
