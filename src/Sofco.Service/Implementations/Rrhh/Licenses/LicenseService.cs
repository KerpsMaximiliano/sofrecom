using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Managers.UserApprovers;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Rrhh;
using Sofco.Core.Services.Rrhh.Licenses;
using Sofco.Core.StatusHandlers;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Utils;
using Sofco.Framework.ValidationHelpers.Rrhh;

namespace Sofco.Service.Implementations.Rrhh.Licenses
{
    public class LicenseService : ILicenseService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<LicenseService> logger;
        private readonly ILicenseStatusFactory licenseStatusFactory;
        private readonly IMailSender mailSender;
        private readonly ILicenseApproverManager licenseApproverManager;
        private readonly ILicenseGenerateWorkTimeService licenseGenerateWorkTimeService;

        public LicenseService(IUnitOfWork unitOfWork, 
                              ILogMailer<LicenseService> logger, 
                              ILicenseStatusFactory licenseStatusFactory, 
                              IMailSender mailSender,
                              ILicenseApproverManager licenseApproverManager,
                              ILicenseGenerateWorkTimeService licenseGenerateWorkTimeService)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.licenseStatusFactory = licenseStatusFactory;
            this.mailSender = mailSender;
            this.licenseApproverManager = licenseApproverManager;
            this.licenseGenerateWorkTimeService = licenseGenerateWorkTimeService;
        }

        public Response<LicenseAddModel> Add(LicenseAddModel model)
        {
            var response = new Response<LicenseAddModel>();

            SetManager(model, response);

            var domain = model.CreateDomain();

            LicenseValidationHandler.ValidateEmployee(response, domain, unitOfWork);
            LicenseValidationHandler.ValidateManager(response, domain, unitOfWork);
            LicenseValidationHandler.ValidateDates(response, domain, model.IsRrhh);
            LicenseValidationHandler.ValidateSector(response, domain);
            LicenseValidationHandler.ValidateLicenseType(response, domain);
            LicenseValidationHandler.ValidateDatesOverlaped(response, domain, unitOfWork);
            LicenseValidationHandler.ValidateWorkTimeOverlap(response, domain, unitOfWork);

            if (response.HasErrors()) return response;

            LicenseValidationHandler.ValidateDays(response, domain, unitOfWork);
            LicenseValidationHandler.ValidateApplicantNotEqualManager(response, domain, unitOfWork);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.LicenseRepository.Insert(domain);
                unitOfWork.Save();

                model.Id = domain.Id;

                response.Data = model;
                response.AddSuccess(Resources.Rrhh.License.SaveSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            var license = LicenseValidationHandler.FindFull(domain.Id, response, unitOfWork);

            try
            {
                // Generates all worktimes between license days
                licenseGenerateWorkTimeService.GenerateWorkTimes(license);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddWarning(Resources.Rrhh.License.GenerateWorkTimesError);
            }

            if (!response.HasErrors())
            {
                UpdateStatus(model, response, domain.Type, license);
            }

            return response;
        }

        public IList<LicenseListModel> GetByStatus(LicenseStatus statusId)
        {
            var licenses = unitOfWork.LicenseRepository.GetByStatus(statusId).ToList();

            return Translate(licenses);
        }

        public IList<LicenseListModel> Search(LicenseSearchParams parameters)
        {
            var licenses = unitOfWork.LicenseRepository.Search(parameters).ToList();

            return Translate(licenses);
        }

        public IList<LicenseListModel> GetByManager(int managerId)
        {
            var licenses = unitOfWork.LicenseRepository.GetByManager(managerId).ToList();

            licenses.AddRange(licenseApproverManager.GetByCurrent());

            return Translate(licenses);
        }

        public IList<LicenseListModel> GetByManagerAndStatus(LicenseStatus statusId, int managerId)
        {
            var licenses = unitOfWork.LicenseRepository.GetByManagerAndStatus(statusId, managerId).ToList();

            licenses.AddRange(licenseApproverManager.GetByCurrentByStatus(statusId));

            return Translate(licenses);
        }

        public IList<LicenseListModel> GetByEmployee(int employeeId)
        {
            var licenses = unitOfWork.LicenseRepository.GetByEmployee(employeeId).ToList();

            return Translate(licenses);
        }

        public Response ChangeStatus(int id, LicenseStatusChangeModel model, License license)
        {
            var response = new Response();
            var closeDates = unitOfWork.CloseDateRepository.GetFirstBeforeNextMonth();

            DateTime closeDate = new DateTime(closeDates.Year,closeDates.Month,closeDates.Day);



            if (license == null)
                license = LicenseValidationHandler.FindFull(id, response, unitOfWork);

            var licenseStatusHandler = licenseStatusFactory.GetInstance(model.Status);
             if (closeDate < license.StartDate) { 
            try
            {
                // Validate Status
                licenseStatusHandler.Validate(response, unitOfWork, model, license);

                if (response.HasErrors()) return response;

                // Update Status
                licenseStatusHandler.SaveStatus(license, model, unitOfWork);

                // Add History
                var history = GetHistory(license, model);
                unitOfWork.LicenseRepository.AddHistory(history);

                // Save
                unitOfWork.Save();
                response.AddSuccess(licenseStatusHandler.GetSuccessMessage());
            }
            catch (Exception e)
            {
                logger.LogError(e);

                if (response.Messages.Any(x => x.Type == MessageType.Success))
                {
                    response.AddWarning(Resources.Rrhh.License.ChangeStatusError);
                }
                else
                {
                    response.AddError(Resources.Rrhh.License.ChangeStatusError);
                }
                
                return response;
            }

            try
            {
                // Remove all worktimes between license days
                if (model.Status == LicenseStatus.Cancelled || model.Status == LicenseStatus.Rejected)
                {
                    unitOfWork.WorkTimeRepository.RemoveBetweenDays(license.EmployeeId, license.StartDate, license.EndDate);
                    unitOfWork.Save();
                }
            }
            catch (Exception e)
            { 
                logger.LogError(e);
                response.AddWarning(Resources.WorkTimeManagement.WorkTime.DeleteError);
            }

            SendMail(license, response, licenseStatusHandler, model);
            }
            else
            {
                response.AddError(Resources.Rrhh.License.ChangeStatusError);
            }

            return response;
        }

        private void SendMail(License license, Response response, ILicenseStatusHandler licenseStatusHandler, LicenseStatusChangeModel parameters)
        {
            var data = licenseStatusHandler.GetEmailData(license, unitOfWork, parameters);

            try
            {
                mailSender.Send(data);
            }
            catch (Exception e)
            {
                var recipients = data.Recipients;

                recipients.Add(data.Recipient);

                var msg = $"Subject: {data.Title} - Recipients: [{string.Join(",", recipients)}]";

                logger.LogError(msg, e);

                response.AddWarning(Resources.Common.ErrorSendMail);
            }
        }

        public Response<LicenseDetailModel> GetById(int id)
        {
            var response = new Response<LicenseDetailModel>();

            var license = LicenseValidationHandler.FindFull(id, response, unitOfWork);

            if (response.HasErrors()) return response;

            response.Data = new LicenseDetailModel(license);

            return response;
        }

        public ICollection<LicenseHistoryModel> GetHistories(int id)
        {
            var histories = unitOfWork.LicenseRepository.GetHistories(id);

            return histories.Select(x => new LicenseHistoryModel(x)).ToList();
        }

        private LicenseHistory GetHistory(License license, LicenseStatusChangeModel model)
        {
            var history = new LicenseHistory
            {
                LicenseStatusFrom = license.Status,
                LicenseStatusTo = model.Status,
                LicenseId = license.Id,
                UserId = model.UserId,
                CreatedDate = DateTime.UtcNow,
                Comment = model.Comment
            };

            return history;
        }

        private void UpdateStatus(LicenseAddModel model, Response<LicenseAddModel> response, LicenseType licenseType, License license)
        {
            if (model.IsRrhh && model.EmployeeLoggedId != model.EmployeeId)
            {
                var statusParams = new LicenseStatusChangeModel
                {
                    Status = licenseType.CertificateRequired ? LicenseStatus.ApprovePending : LicenseStatus.Approved,
                    UserId = model.UserId,
                    IsRrhh = model.IsRrhh
                };

                var statusResponse = ChangeStatus(model.Id, statusParams, license);
                response.AddMessages(statusResponse.Messages);
            }
            else
            {
                var statusParams = new LicenseStatusChangeModel
                {
                    Status = LicenseStatus.AuthPending,
                    UserId = model.UserId,
                    IsRrhh = model.IsRrhh
                };

                var statusResponse = ChangeStatus(model.Id, statusParams, license);
                response.AddMessages(statusResponse.Messages);
            }
        }

        private List<LicenseListModel> Translate(List<License> licenses)
        {
            var result = licenses
                .Select(x => new LicenseListModel(x))
                .ToList();

            return licenseApproverManager.ResolveApprovers(result);
        }

        private void SetManager(LicenseAddModel license, Response response)
        {
            var employee = unitOfWork.EmployeeRepository.GetById(license.EmployeeId);

            if (employee.ManagerId != null && license.ManagerId != employee.ManagerId.Value)
            {
                license.ManagerDesc = unitOfWork.UserRepository.GetUserLiteById(employee.ManagerId.Value).Name;
            }

            if (employee.ManagerId != null) license.ManagerId = employee.ManagerId.Value;
        }
    }
}
