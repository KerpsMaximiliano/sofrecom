using System.Collections.Generic;
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
    public class LicenseStatusApproveHandler : ILicenseStatusHandler
    {
        private readonly EmailConfig emailConfig;

        private readonly ILicenseApproverManager licenseApproverManager;

        public LicenseStatusApproveHandler(EmailConfig emailConfig, ILicenseApproverManager licenseApproverManager)
        {
            this.emailConfig = emailConfig;
            this.licenseApproverManager = licenseApproverManager;
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

                if (license.TypeId == 19 && license.Employee.BirthdayDaysTaken + license.DaysQuantity > license.Type.Days)
                {
                    response.AddError(Resources.Rrhh.License.BirthdayDaysTakenExceeded);
                }

                if (license.TypeId == 20 && license.Employee.FlexDaysTaken + license.DaysQuantity > license.Type.Days)
                {
                    response.AddError(Resources.Rrhh.License.FlexDaysTakenExceeded);
                }

                if (license.TypeId == 18 && license.Employee.PaternityDaysTaken + license.DaysQuantity > license.Type.Days)
                {
                    response.AddError(Resources.Rrhh.License.PaternityDaysTakenExceeded);
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

            if (license.TypeId == 18)
            {
                license.Employee.PaternityDaysTaken += license.DaysQuantity;
                unitOfWork.EmployeeRepository.Update(license.Employee);
            }

            if (license.TypeId == 19)
            {
                license.Employee.BirthdayDaysTaken += license.DaysQuantity;
                unitOfWork.EmployeeRepository.Update(license.Employee);
            }            

            if (license.TypeId == 20)
            {
                license.Employee.FlexDaysTaken += license.DaysQuantity;
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
            var body = string.Format(MailMessageResource.LicenseApproveMessage, 
                $"{emailConfig.SiteUrl}rrhh/licenses/{license.Id}/detail",
                license.Type.Description,
                GetComments(license));

            var mailRrhh = unitOfWork.GroupRepository.GetEmail(emailConfig.RrhhCode);

            var recipientsList = new List<string> { mailRrhh, license.Manager.Email, license.Employee.Email };

            recipientsList.AddRange(licenseApproverManager.GetEmailApproversByEmployeeId(license.EmployeeId));

            var data = new LicenseStatusData
            {
                Title = subject,
                Message = body,
                Recipients = recipientsList
            };

            return data;
        }

        private string GetComments(Domain.Models.Rrhh.License license)
        {
            if (string.IsNullOrEmpty(license.Comments)) return string.Empty;

            return MailCommonResource.Comments + ": " + license.Comments + "<br/><br/>";
        }
    }
}

