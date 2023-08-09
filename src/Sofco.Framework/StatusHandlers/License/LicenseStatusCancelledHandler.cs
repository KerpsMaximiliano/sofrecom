using System.Collections.Generic;
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
using Sofco.Core.Data.Admin;
using System;

namespace Sofco.Framework.StatusHandlers.License
{
    public class LicenseStatusCancelledHandler : ILicenseStatusHandler
    {
        private readonly EmailConfig emailConfig;

        private readonly ILicenseApproverManager licenseApproverManager;
		private readonly IUserData userData;

		public LicenseStatusCancelledHandler(EmailConfig emailConfig, ILicenseApproverManager licenseApproverManager, IUserData userData)
        {
            this.emailConfig = emailConfig;
            this.licenseApproverManager = licenseApproverManager;
			this.userData = userData ?? throw new System.ArgumentNullException(nameof(userData));
		}

        public void Validate(Response response, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters, Domain.Models.Rrhh.License license)
        {

            var closeDates = unitOfWork.CloseDateRepository.GetFirstBeforeNextMonth();
            DateTime closeDate = new DateTime(closeDates.Year, closeDates.Month, closeDates.Day);

            if (license.StartDate < closeDate)
            {
                response.AddError(Resources.Rrhh.License.periodBefore);
            } else { 

                var currentUser = userData.GetCurrentUser();
                var permiso = unitOfWork.UserRepository.HasPermission(currentUser.Id, "REMOV", "CTRLI");
                if (!permiso)
                {
                    response.AddErrorAndNoTraslate("No tiene permisos para cancelar la licencia");
                }
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

            //Status = 7 (Cancelado)
            if(licenseToModif.Status == LicenseStatus.Cancelled)
            {
                //Borramos todos los WorkTime de la Licencia
                DeleteWorkTimeByLicense(license, unitOfWork);
            }

            if (license.TypeId == 7)
            {
                license.Employee.ExamDaysTaken -= license.DaysQuantity;
                unitOfWork.EmployeeRepository.Update(license.Employee);
            }

            if (license.TypeId == 17)
            {
                license.Employee.PaternityDaysTaken -= license.DaysQuantity;
                unitOfWork.EmployeeRepository.Update(license.Employee);
            }

            if (license.TypeId == 18)
            {
                license.Employee.BirthdayDaysTaken -= license.DaysQuantity;
                unitOfWork.EmployeeRepository.Update(license.Employee);
            }            

            if (license.TypeId == 19)
            {
                license.Employee.FlexDaysTaken -= license.DaysQuantity;
                unitOfWork.EmployeeRepository.Update(license.Employee);
            }
        }
        private void DeleteWorkTimeByLicense(Domain.Models.Rrhh.License license, IUnitOfWork unitOfWork)
        {
            var listWorkTimes = unitOfWork.WorkTimeRepository.GetByEmployeeId(license.StartDate, license.EndDate, license.EmployeeId);

            for(int i = 0; i < listWorkTimes.Count;i++)
            {
                unitOfWork.WorkTimeRepository.Delete(listWorkTimes[i]);
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
