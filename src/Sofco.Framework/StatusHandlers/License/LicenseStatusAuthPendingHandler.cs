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
    public class LicenseStatusAuthPendingHandler : ILicenseStatusHandler
    {
        private readonly EmailConfig emailConfig;

        private readonly ILicenseApproverManager licenseApproverManager;

        public LicenseStatusAuthPendingHandler(EmailConfig emailConfig, ILicenseApproverManager licenseApproverManager)
        {
            this.emailConfig = emailConfig;
            this.licenseApproverManager = licenseApproverManager;
        }

        public void Validate(Response response, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters, Domain.Models.Rrhh.License license)
        {
            if (license.Status != LicenseStatus.Draft)
            {
                response.AddError(Resources.Rrhh.License.CannotChangeStatus);
            }
        }

        public void SaveStatus(Domain.Models.Rrhh.License license, LicenseStatusChangeModel model, IUnitOfWork unitOfWork)
        {
            var licenseToModif = new Domain.Models.Rrhh.License { Id = license.Id, Status = model.Status };
            unitOfWork.LicenseRepository.UpdateStatus(licenseToModif);
        }

        public string GetSuccessMessage()
        {
            return Resources.Rrhh.License.AuthPendingSuccess;
        }

        public IMailData GetEmailData(Domain.Models.Rrhh.License license, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters)
        {
            var subject = string.Format(MailSubjectResource.LicenseWorkflowTitle, license.Employee.Name);
            var body = string.Format(MailMessageResource.LicenseAuthPendingMessage, $"{emailConfig.SiteUrl}rrhh/licenses/{license.Id}/detail", license.Employee.Name, license.Type.Description);

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
    }
}
 