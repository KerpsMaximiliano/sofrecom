using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Framework.MailData;
using Sofco.Model.Enums.TimeManagement;
using Sofco.Model.Utils;
using Sofco.Resources.Mails;

namespace Sofco.Framework.StatusHandlers.Analytic
{
    public static class AnalyticStatusClose
    {
        public static void Save(Model.Models.AllocationManagement.Analytic analytic, IUnitOfWork unitOfWork, Response response)
        {
            unitOfWork.AllocationRepository.RemoveAllocationByAnalytic(analytic.Id, new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));

            analytic.Status = AnalyticStatus.Close;
            unitOfWork.AnalyticRepository.Close(analytic);

            unitOfWork.Save();

            response.AddSuccess(Resources.AllocationManagement.Analytic.CloseSuccess);
        }

        public static void SendMail(Response response, Model.Models.AllocationManagement.Analytic analytic, EmailConfig emailConfig, IMailSender mailSender, IUnitOfWork unitOfWork, IMailBuilder mailBuilder)
        {
            var recipientsList = new List<string>();

            var mailPmo = unitOfWork.GroupRepository.GetEmail(emailConfig.PmoCode);
            var mailDaf = unitOfWork.GroupRepository.GetEmail(emailConfig.DafCode);
            var mailRrhh = unitOfWork.GroupRepository.GetEmail(emailConfig.RrhhCode);

            recipientsList.AddRange(new[] { mailPmo, mailRrhh, mailDaf });

            var manager = unitOfWork.UserRepository.GetSingle(x => x.Id == analytic.ManagerId);

            if (manager != null) recipientsList.Add(manager.Email);

            var recipients = string.Join(";", recipientsList.Distinct());

            var data = new CloseAnalyticData
            {
                Title = string.Format(MailSubjectResource.CloseAnalytic, analytic.ClientExternalName),
                Message = string.Format(MailMessageResource.CloseAnalytic, $"{analytic.Title} - {analytic.Name}", analytic.Service),
                Recipients = recipients
            };

            var email = mailBuilder.GetEmail(data);

            mailSender.Send(email);
        }
    }
}
