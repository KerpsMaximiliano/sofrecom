using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
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
    public class LicenseStatusRejectHandler : ILicenseStatusHandler
    {
        private readonly EmailConfig emailConfig;

        private readonly ILicenseApproverManager licenseApproverManager;

        private readonly IUserData userData;

        public LicenseStatusRejectHandler(EmailConfig emailConfig, ILicenseApproverManager licenseApproverManager, IUserData userData)
        {
            this.emailConfig = emailConfig;
            this.licenseApproverManager = licenseApproverManager;
            this.userData = userData;
        }

        public void Validate(Response response, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters, Domain.Models.Rrhh.License license)
        {
            // Si esta en borrador o aprobada no se puede rechazar
            if (license.Status == LicenseStatus.Draft || license.Status == LicenseStatus.Approved)
            {
                response.AddError(Resources.Rrhh.License.CannotChangeStatus);
            }

            var currentUser = userData.GetCurrentUser();

            if (license.ManagerId != currentUser.Id)
            {
                var delegations = unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(currentUser.Id, DelegationType.LicenseAuthorizer);

                if (!delegations.Any(x =>
                    x.UserId == license.ManagerId && x.EmployeeSourceId.GetValueOrDefault() == license.EmployeeId))
                {
                    response.AddErrorAndNoTraslate("No tiene permisos para autorizar la licencia");
                }
            }
        }

        public void SaveStatus(Domain.Models.Rrhh.License license, LicenseStatusChangeModel model, IUnitOfWork unitOfWork)
        {
            var licenseToModif = new Domain.Models.Rrhh.License { Id = license.Id, Status = model.Status };
            unitOfWork.LicenseRepository.UpdateStatus(licenseToModif);
        }

        public string GetSuccessMessage()
        {
            return Resources.Rrhh.License.RejectSuccess;
        }

        public IMailData GetEmailData(Domain.Models.Rrhh.License license, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters)
        {
            var subject = string.Format(MailSubjectResource.LicenseWorkflowTitle, license.Employee.Name);

            var body = string.Format(MailMessageResource.LicenseRejectMessage, $"{emailConfig.SiteUrl}rrhh/licenses/{license.Id}/detail", parameters.Comment, license.Type.Description);

            var mailRrhh = unitOfWork.GroupRepository.GetEmail(emailConfig.RrhhCode);

            var recipients = new List<string> { mailRrhh, license.Manager.Email, license.Employee.Email };

            recipients.AddRange(licenseApproverManager.GetEmailApproversByEmployeeId(license.EmployeeId));

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
