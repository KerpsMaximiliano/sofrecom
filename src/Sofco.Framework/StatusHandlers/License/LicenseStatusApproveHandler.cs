using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Resources.Mails;

namespace Sofco.Framework.StatusHandlers.License
{
    public class LicenseStatusApproveHandler : ILicenseStatusHandler
    {
        private readonly EmailConfig emailConfig;

        public LicenseStatusApproveHandler(EmailConfig emailConfig)
        {
            this.emailConfig = emailConfig;
        }

        public void Validate(Response response, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters, Domain.Models.Rrhh.License license)
        {
            if (!parameters.IsRrhh) response.AddError(Resources.Rrhh.License.CannotChangeStatus);

            if (parameters.IsRrhh)
            {
                if (license.Status == LicenseStatus.AuthPending || license.Status == LicenseStatus.Rejected)
                {
                    response.AddError(Resources.Rrhh.License.CannotChangeStatus);
                }

                if (license.Status == LicenseStatus.Draft && license.Type.CertificateRequired)
                {
                    response.AddError(Resources.Rrhh.License.CannotChangeStatus);
                }

                if (license.TypeId == 1 && license.Employee.HolidaysPending - license.DaysQuantity < 0)
                {
                    response.AddError(Resources.Rrhh.License.DaysWrong);
                }

                if (license.TypeId == 7 && license.Employee.ExamDaysTaken + license.DaysQuantity > license.Type.Days)
                {
                    response.AddError(Resources.Rrhh.License.ExamDaysTakenExceeded);
                }
            }
        }

        public void SaveStatus(Domain.Models.Rrhh.License license, LicenseStatusChangeModel model, IUnitOfWork unitOfWork)
        {
            var licenseToModif = new Domain.Models.Rrhh.License { Id = license.Id, Status = model.Status };
            unitOfWork.LicenseRepository.UpdateStatus(licenseToModif);

            if (license.TypeId == 1)
            {
                license.Employee.HolidaysPending -= license.DaysQuantity;

                if (license.Employee.HolidaysPending == 0)
                {
                    license.Employee.HolidaysPendingByLaw = 0;
                }
                else
                {
                    license.Employee.HolidaysPendingByLaw -= license.DaysQuantityByLaw;
                }

                unitOfWork.EmployeeRepository.Update(license.Employee);
            }

            if (license.TypeId == 7)
            {
                license.Employee.ExamDaysTaken += license.DaysQuantity;
                unitOfWork.EmployeeRepository.Update(license.Employee);
            }
        }

        public string GetSuccessMessage()
        {
            return Resources.Rrhh.License.ApproveSuccess;
        }

        public IMailData GetEmailData(Domain.Models.Rrhh.License license, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters)
        {
            var subject = string.Format(MailSubjectResource.LicenseWorkflowTitle, license.Employee.Name);
            var body = string.Format(MailMessageResource.LicenseApproveMessage, $"{emailConfig.SiteUrl}rrhh/licenses/{license.Id}/detail", license.Type.Description);

            var mailRrhh = unitOfWork.GroupRepository.GetEmail(emailConfig.RrhhCode);

            var recipientsList = new List<string> { mailRrhh, license.Manager.Email, license.Employee.Email };

            var recipients = string.Join(";", recipientsList.Distinct());

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

