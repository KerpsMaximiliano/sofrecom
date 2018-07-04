using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Model.Enums;
using Sofco.Model.Utils;
using Sofco.Resources.Mails;

namespace Sofco.Framework.StatusHandlers.License
{
    public class LicenseStatusApprovePendingHandler : ILicenseStatusHandler
    {
        private readonly EmailConfig emailConfig;

        public LicenseStatusApprovePendingHandler(EmailConfig emailConfig)
        {
            this.emailConfig = emailConfig;
        }

        public void Validate(Response response, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters, Model.Models.Rrhh.License license)
        {
            if (!parameters.IsRrhh) response.AddError(Resources.Rrhh.License.CannotChangeStatus);

            if (license.Status != LicenseStatus.Pending && license.Status != LicenseStatus.Draft)
            {
                response.AddError(Resources.Rrhh.License.CannotChangeStatus);
            }
        }

        public void SaveStatus(Model.Models.Rrhh.License license, LicenseStatusChangeModel model, IUnitOfWork unitOfWork)
        {
            var licenseToModif = new Model.Models.Rrhh.License { Id = license.Id, Status = model.Status };
            unitOfWork.LicenseRepository.UpdateStatus(licenseToModif);
        }

        public string GetSuccessMessage()
        {
            return Resources.Rrhh.License.AuthPendingSuccess;
        }

        public IMailData GetEmailData(Model.Models.Rrhh.License license, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters)
        {
            var subject = string.Format(MailSubjectResource.LicenseWorkflowTitle, license.Employee.Name);
            var body = string.Format(MailMessageResource.LicenseApprovePendingMessage, $"{emailConfig.SiteUrl}rrhh/licenses/{license.Id}/detail", license.Type.Description);

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
