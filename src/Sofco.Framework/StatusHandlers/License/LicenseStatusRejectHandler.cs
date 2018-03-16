using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;
using Sofco.Resources.Mails;

namespace Sofco.Framework.StatusHandlers.License
{
    public class LicenseStatusRejectHandler : ILicenseStatusHandler
    {
        private readonly EmailConfig emailConfig;

        public LicenseStatusRejectHandler(EmailConfig emailConfig)
        {
            this.emailConfig = emailConfig;
        }

        public void Validate(Response response, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters, Model.Models.Rrhh.License license)
        {
            if(!parameters.IsRrhh) response.AddError(Resources.Rrhh.License.CannotChangeStatus);

            if (license.Status == LicenseStatus.Draft || license.Status == LicenseStatus.Approved)
            {
                response.AddError(Resources.Rrhh.License.CannotChangeStatus);
            }
        }

        public void SaveStatus(Model.Models.Rrhh.License license, LicenseStatusChangeModel model, IUnitOfWork unitOfWork)
        {
            var licenseToModif = new Model.Models.Rrhh.License { Id = license.Id, Status = model.Status };
            unitOfWork.LicenseRepository.UpdateStatus(licenseToModif);

            if (license.TypeId == 1)
            {
                var employeeToModif = new Employee { Id = license.EmployeeId, HolidaysPending = license.Employee.HolidaysPending + license.DaysQuantity };
                unitOfWork.EmployeeRepository.UpdateHolidaysPending(employeeToModif);
            }

            if (license.TypeId == 7)
            {
                if (license.Employee.ExamDaysTaken - license.DaysQuantity >= 0)
                {
                    var employeeToModif = new Employee { Id = license.EmployeeId, ExamDaysTaken = license.Employee.ExamDaysTaken - license.DaysQuantity };
                    unitOfWork.EmployeeRepository.UpdateExamDaysTaken(employeeToModif);
                } 
            }
        }

        public string GetSuccessMessage()
        {
            return Resources.Rrhh.License.RejectSuccess;
        }

        public IMailData GetEmailData(Model.Models.Rrhh.License license, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters)
        {
            var subject = string.Format(MailSubjectResource.LicenseWorkflowTitle, license.Employee.Name);
            var body = string.Format(MailMessageResource.LicenseRejectMessage, $"{emailConfig.SiteUrl}allocationManagement/licenses/{license.Id}/detail", parameters.Comment);

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
