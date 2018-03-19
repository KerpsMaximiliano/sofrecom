﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Rrhh;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.ValidationHelpers.Rrhh;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Rrhh;
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

        public LicenseService(IUnitOfWork unitOfWork, ILogMailer<LicenseService> logger, IOptions<FileConfig> fileOptions, 
                              ISessionManager sessionManager, ILicenseStatusFactory licenseStatusFactory, IMailBuilder mailBuilder, 
                              IMailSender mailSender)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.fileConfig = fileOptions.Value;
            this.sessionManager = sessionManager;
            this.licenseStatusFactory = licenseStatusFactory;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
        }

        public Response<string> Add(License domain)
        {
            var response = new Response<string>();

            LicenseValidationHandler.ValidateEmployee(response, domain, unitOfWork);
            LicenseValidationHandler.ValidateManager(response, domain, unitOfWork);
            LicenseValidationHandler.ValidateDates(response, domain);
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

        public Response ChangeStatus(int id, LicenseStatusChangeModel model)
        {
            var response = new Response();

            var license = LicenseValidationHandler.FindFull(id, response, unitOfWork);
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

                // Save4
                unitOfWork.Save();
                response.AddSuccess(licenseStatusHandler.GetSuccessMessage());
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            SendMail(license, response, licenseStatusHandler, model);

            return response;
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
    }
}
