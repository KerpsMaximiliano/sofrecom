using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Rrhh.Licenses;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;
using Sofco.Framework.ValidationHelpers.Rrhh;
using File = Sofco.Domain.Models.Common.File;

namespace Sofco.Service.Implementations.Rrhh.Licenses
{
    public class LicenseFileService : ILicenseFileService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<LicenseFileService> logger;
        private readonly FileConfig fileConfig;
        private readonly ISessionManager sessionManager;
        private readonly ILicenseFileManager licenseFileManager;

        public LicenseFileService(IUnitOfWork unitOfWork, 
            ILogMailer<LicenseFileService> logger, 
            IOptions<FileConfig> fileOptions,
            ISessionManager sessionManager, 
            ILicenseFileManager licenseFileManager)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            fileConfig = fileOptions.Value;
            this.sessionManager = sessionManager;
            this.licenseFileManager = licenseFileManager;
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

        public Response<byte[]> GetLicenseReport(ReportParams parameters)
        {
            var response = new Response<byte[]>();

            if (parameters.StartDate == DateTime.MinValue || parameters.EndDate == DateTime.MinValue)
            {
                response.AddError(Resources.Rrhh.License.DatesRequired);
            }
            else if (parameters.StartDate.Date > parameters.EndDate.Date)
            {
                response.AddError(Resources.Rrhh.License.EndDateLessThanStartDate);
            }

            if (response.HasErrors()) return response;

            var licenses = unitOfWork.LicenseRepository.GetLicensesReport(parameters);

            if (!licenses.Any())
            {
                response.AddError(Resources.Rrhh.License.ReportWithoutEmpty);
                return response;
            }

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
    }
}
