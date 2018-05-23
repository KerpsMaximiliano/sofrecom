using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Rrhh;
using Sofco.Core.StatusHandlers;
using Sofco.Data.Admin;
using Sofco.Data.AllocationManagement;
using Sofco.Framework.ValidationHelpers.Rrhh;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Models.Rrhh;
using Sofco.Model.Models.WorkTimeManagement;
using Sofco.Model.Relationships;
using Sofco.Model.Utils;
using File = Sofco.Model.Models.Common.File;

namespace Sofco.Service.Implementations.Rrhh
{
    public class LicenseService : ILicenseService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<LicenseService> logger;
        private readonly FileConfig fileConfig;
        private readonly ISessionManager sessionManager;
        private readonly ILicenseStatusFactory licenseStatusFactory;
        private readonly IMailBuilder mailBuilder;
        private readonly IMailSender mailSender;
        private readonly ILicenseFileManager licenseFileManager;

        public LicenseService(IUnitOfWork unitOfWork, ILogMailer<LicenseService> logger, IOptions<FileConfig> fileOptions, 
                              ISessionManager sessionManager, ILicenseStatusFactory licenseStatusFactory, IMailBuilder mailBuilder, 
                              IMailSender mailSender, ILicenseFileManager licenseFileManager)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.fileConfig = fileOptions.Value;
            this.sessionManager = sessionManager;
            this.licenseStatusFactory = licenseStatusFactory;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
            this.licenseFileManager = licenseFileManager;
        }

        public Response<string> Add(LicenseAddModel model)
        {
            var response = new Response<string>();
            var domain = model.CreateDomain();

            LicenseValidationHandler.ValidateEmployee(response, domain, unitOfWork);
            LicenseValidationHandler.ValidateManager(response, domain, unitOfWork);
            LicenseValidationHandler.ValidateDates(response, domain, model.IsRrhh);
            LicenseValidationHandler.ValidateSector(response, domain);
            LicenseValidationHandler.ValidateLicenseType(response, domain);

            if (response.HasErrors()) return response;

            LicenseValidationHandler.ValidateDays(response, domain, unitOfWork);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.LicenseRepository.Insert(domain);
                unitOfWork.Save();

                response.Data = domain.Id.ToString();
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
                if (license.Status == LicenseStatus.Draft)
                {
                    GenerateWorkTimes(license);
                    unitOfWork.Save();
                }
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

        public async Task<Response<File>> AttachFile(int id, Response<File> response, IFormFile file)
        {
            LicenseValidationHandler.Find(id, response, unitOfWork);

            if (response.HasErrors()) return response;

            var fileToAdd = new File();
            var lastDotIndex = file.FileName.LastIndexOf('.');

            fileToAdd.FileName = file.FileName;
            fileToAdd.FileType = file.FileName.Substring(lastDotIndex);
            fileToAdd.InternalFileName = Guid.NewGuid();
            fileToAdd.CreationDate = DateTime.UtcNow;
            fileToAdd.CreatedUser = sessionManager.GetUserName();

            try
            {
                var fileName = $"{fileToAdd.InternalFileName.ToString()}{fileToAdd.FileType}";

                using (var fileStream = new FileStream(Path.Combine(fileConfig.LicensesPath, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                unitOfWork.FileRepository.Insert(fileToAdd);
                unitOfWork.Save();

                var licenseFile = new LicenseFile { FileId = fileToAdd.Id, LicenseId = id };
                unitOfWork.LicenseRepository.AddFile(licenseFile);
                unitOfWork.Save();

                response.Data = fileToAdd;
                response.AddSuccess(Resources.Rrhh.License.FileAdded);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.SaveFileError);
                logger.LogError(e);
            }

            return response;
        }

        public IList<LicenseListItem> GetByStatus(LicenseStatus statusId)
        {
            var licenses = unitOfWork.LicenseRepository.GetByStatus(statusId);

            return licenses.Select(x => new LicenseListItem(x)).ToList();
        }

        public IList<LicenseListItem> Search(LicenseSearchParams parameters)
        {
            var licenses = unitOfWork.LicenseRepository.Search(parameters);

            return licenses.Select(x => new LicenseListItem(x)).ToList();
        }

        public IList<LicenseListItem> GetByManager(int managerId)
        {
            var licenses = unitOfWork.LicenseRepository.GetByManager(managerId);

            return licenses.Select(x => new LicenseListItem(x)).ToList();
        }

        public IList<LicenseListItem> GetByManagerAndStatus(LicenseStatus statusId, int managerId)
        {
            var licenses = unitOfWork.LicenseRepository.GetByManagerAndStatus(statusId, managerId);

            return licenses.Select(x => new LicenseListItem(x)).ToList();
        }

        public IList<LicenseListItem> GetByEmployee(int employeeId)
        {
            var licenses = unitOfWork.LicenseRepository.GetByEmployee(employeeId);

            return licenses.Select(x => new LicenseListItem(x)).ToList();
        }

        public Response DeleteFile(int id)
        {
            var response = new Response();

            var licenseFile = unitOfWork.FileRepository.GetSingle(x => x.Id == id);

            if (licenseFile == null)
            {
                response.AddError(Resources.Common.FileNotFound);
                return response;
            }

            try
            {
                unitOfWork.FileRepository.Delete(licenseFile);

                var fileName = $"{licenseFile.InternalFileName.ToString()}{licenseFile.FileType}";
                var path = Path.Combine(fileConfig.LicensesPath, fileName);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                unitOfWork.Save();
                response.AddSuccess(Resources.Common.FileDeleted);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.GeneralError);
                logger.LogError(e);
            }

            return response;
        }

        public Response ChangeStatus(int id, LicenseStatusChangeModel model, License license)
        {
            var response = new Response();

            if (license == null)
                license = LicenseValidationHandler.FindFull(id, response, unitOfWork);

            var licenseStatusHandler = licenseStatusFactory.GetInstance(model.Status);

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
                }
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddWarning(Resources.WorkTimeManagement.WorkTime.DeleteError);
            }

            SendMail(license, response, licenseStatusHandler, model);

            return response;
        }

        private void GenerateWorkTimes(License license)
        {
            var startDate = license.StartDate;
            var endDate = license.EndDate;

            var allocationStartDate = new DateTime(startDate.Year, startDate.Month, 1);
            var allocationEndDate = new DateTime(endDate.Year, endDate.Month, 1);

            var allocations = unitOfWork.AllocationRepository.GetAllocationsLiteBetweenDays(license.EmployeeId, allocationStartDate, allocationEndDate);

            var user = unitOfWork.UserRepository.GetByEmail(license.Employee.Email);

            var analyticBank = unitOfWork.AnalyticRepository.GetByTitle("492-00000");

            while (startDate.Date <= endDate.Date)
            {
                if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    startDate = startDate.AddDays(1);
                    continue;
                }

                var startDateOfMonth = new DateTime(startDate.Year, startDate.Month, 1);

                var allocationsInMonth = allocations.Where(x => x.StartDate == startDateOfMonth).ToList();

                if (!allocationsInMonth.Any())
                {
                    var worktime = BuildWorkTime(license, startDate, user);

                    worktime.AnalyticId = analyticBank.Id;
                    worktime.Hours = 8;

                    unitOfWork.WorkTimeRepository.Insert(worktime);
                }
                else
                {
                    if (allocationsInMonth.All(x => x.Percentage == 0))
                    {
                        var worktime = BuildWorkTime(license, startDate, user);

                        worktime.AnalyticId = analyticBank.Id;
                        worktime.Hours = 8;

                        unitOfWork.WorkTimeRepository.Insert(worktime);
                    }
                    else
                    {
                        foreach (var allocation in allocationsInMonth)
                        {
                            if (allocation.Percentage > 0)
                            {
                                var worktime = BuildWorkTime(license, startDate, user);

                                worktime.AnalyticId = allocation.AnalyticId;
                                worktime.Hours = (8 * allocation.Percentage) / 100;

                                unitOfWork.WorkTimeRepository.Insert(worktime);
                            }
                        }
                    }
                }

                startDate = startDate.AddDays(1);
            }
        }

        private WorkTime BuildWorkTime(License license, DateTime startDate, Model.Models.Admin.User user)
        {
            var worktime = new WorkTime();

            worktime.EmployeeId = license.EmployeeId;
            worktime.UserId = user.Id;
            worktime.UserComment = license.Type.Description;
            worktime.CreationDate = DateTime.UtcNow.Date;
            worktime.Status = WorkTimeStatus.License;
            worktime.Date = startDate.Date;
            worktime.TaskId = license.Type.TaskId;

            return worktime;
        }

        private void SendMail(License license, Response response, ILicenseStatusHandler licenseStatusHandler, LicenseStatusChangeModel parameters)
        {
            try
            {
                var data = licenseStatusHandler.GetEmailData(license, unitOfWork, parameters);
                var email = mailBuilder.GetEmail(data);
                mailSender.Send(email);
            }
            catch (Exception e)
            {
                logger.LogError(e);
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

        public Response<byte[]> GetLicenseReport(ReportParams parameters)
        {
            var response = new Response<byte[]>();

            if (parameters.StartDate == DateTime.MinValue || parameters.EndDate == DateTime.MinValue)
            {
                response.AddError(Resources.Rrhh.License.DatesRequired);
            }
            else if(parameters.StartDate.Date > parameters.EndDate.Date)
            {
                response.AddError(Resources.Rrhh.License.EndDateLessThanStartDate);
            }

            if (response.HasErrors()) return response;
 
            var licenses = unitOfWork.LicenseRepository.GetLicensesReport(parameters);

            var excel = licenseFileManager.CreateLicenseReportExcel(licenses);

            response.Data = excel.GetAsByteArray();

            return response;
        }

        public Response FileDelivered(int id)
        {
            var response = new Response();

            var license = LicenseValidationHandler.Find(id, response, unitOfWork);

            if (response.HasErrors()) return response;

            try
            {
                license.HasCertificate = true;
                unitOfWork.LicenseRepository.Update(license);
                unitOfWork.Save();

                response.AddSuccess(Resources.Rrhh.License.UpdateSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void UpdateStatus(LicenseAddModel model, Response<string> response, LicenseType licenseType, License license)
        {
            if (model.IsRrhh && model.EmployeeLoggedId != model.EmployeeId)
            {
                var statusParams = new LicenseStatusChangeModel
                {
                    Status = licenseType.CertificateRequired ? LicenseStatus.ApprovePending : LicenseStatus.Approved,
                    UserId = model.UserId,
                    IsRrhh = model.IsRrhh
                };

                var statusResponse = ChangeStatus(Convert.ToInt32(response.Data), statusParams, license);
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

                var statusResponse = ChangeStatus(Convert.ToInt32(response.Data), statusParams, license);
                response.AddMessages(statusResponse.Messages);
            }
        }
    }
}
