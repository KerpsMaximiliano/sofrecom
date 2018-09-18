﻿using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.Managers.UserApprovers;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Resources.Mails;

namespace Sofco.Framework.StatusHandlers.License
{
    public class LicenseStatusCancelledHandler : ILicenseStatusHandler
    {
        private readonly EmailConfig emailConfig;

        private readonly ILicenseApproverManager licenseApproverManager;

        public LicenseStatusCancelledHandler(EmailConfig emailConfig, ILicenseApproverManager licenseApproverManager)
        {
            this.emailConfig = emailConfig;
            this.licenseApproverManager = licenseApproverManager;
        }

        public void Validate(Response response, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters, Domain.Models.Rrhh.License license)
        {
            if (!parameters.IsRrhh) response.AddError(Resources.Rrhh.License.CannotChangeStatus);

            if (parameters.IsRrhh && license.Status != LicenseStatus.Approved)
            {
                response.AddError(Resources.Rrhh.License.CannotChangeStatus);
            }
        }

        public void SaveStatus(Domain.Models.Rrhh.License license, LicenseStatusChangeModel model, IUnitOfWork unitOfWork)
        {
            var licenseToModif = new Domain.Models.Rrhh.License { Id = license.Id, Status = model.Status };
            unitOfWork.LicenseRepository.UpdateStatus(licenseToModif);

            if (license.TypeId == 1)
            {
                license.Employee.HolidaysPending += license.DaysQuantity;
                license.Employee.HolidaysPendingByLaw += license.DaysQuantityByLaw;

                unitOfWork.EmployeeRepository.Update(license.Employee);
            }

            if (license.TypeId == 7)
            {
                license.Employee.ExamDaysTaken -= license.DaysQuantity;
                unitOfWork.EmployeeRepository.Update(license.Employee);
            }
        }

        public string GetSuccessMessage()
        {
            return Resources.Rrhh.License.CancelledSuccess;
        }

        public IMailData GetEmailData(Domain.Models.Rrhh.License license, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters)
        {
            var subject = string.Format(MailSubjectResource.LicenseCancelledTitle, license.Employee.Name);
            var body = string.Format(MailMessageResource.LicenseCancelledMessage, $"{emailConfig.SiteUrl}rrhh/licenses/{license.Id}/detail", parameters.Comment, license.Type.Description);

            var mailRrhh = unitOfWork.GroupRepository.GetEmail(emailConfig.RrhhCode);

            var recipientsList = new List<string> { mailRrhh, license.Manager.Email, license.Employee.Email };

            var recipients = string.Join(";", recipientsList.Distinct());

            recipientsList.AddRange(licenseApproverManager.GetEmailApproversByEmployeeId(license.EmployeeId));

            var data = new LicenseStatusData
            {
                Title = subject,
                Message = body,
                Recipients = recipients
            };

            return data;
        }
    }
}
