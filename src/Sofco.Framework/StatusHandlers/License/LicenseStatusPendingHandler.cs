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
    public class LicenseStatusPendingHandler : ILicenseStatusHandler
    {
        private readonly EmailConfig emailConfig;

        private readonly ILicenseApproverManager licenseApproverManager;

        private readonly IUserData userData;

        public LicenseStatusPendingHandler(EmailConfig emailConfig, ILicenseApproverManager licenseApproverManager, IUserData userData)
        {
            this.emailConfig = emailConfig;
            this.licenseApproverManager = licenseApproverManager;
            this.userData = userData;
        }

        public void Validate(Response response, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters, Domain.Models.Rrhh.License license)
        {
            if (license.Status != LicenseStatus.AuthPending)
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
            return Resources.Rrhh.License.PendingSuccess;
        }

        public IMailData GetEmailData(Domain.Models.Rrhh.License license, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters)
        {
            var subject = string.Format(MailSubjectResource.LicenseWorkflowTitle, license.Employee.Name);
            var body = string.Format(MailMessageResource.LicensePendingMessage, 
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
